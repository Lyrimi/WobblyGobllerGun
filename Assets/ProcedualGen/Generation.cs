using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;

using static RoomTypes;
public class Generation : MonoBehaviour
{
    [SerializeField]

    GenData _genData;
    List<RoomData> _validRooms;


    [SerializeField]
    bool _debugButton;

    [SerializeField]
    bool _debugButton2;

    [SerializeField]
    ContactFilter2D _filter;

    public static System.Random GenRnd;

    int _roomsBuild = 0;

    private Dictionary<RoomType, int> _quotas;

    [SerializeField]

    private List<RoomData> _curRooms;

    private int _totalQouta;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (GenRnd == null)
        {
            GenRnd = new();
        }

        _quotas = _genData.BuildDict();
        foreach (var item in _quotas)
        {
            if (item.Value == -1)
            {
                continue;
            }
            _totalQouta += item.Value;
        }

        _validRooms = _curRooms.ToList();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_debugButton)
        {
            _debugButton = _debugButton;
            Step();
        }

        if (_debugButton2)
        {
            _debugButton2 = false;
            Cleanup();
        }
    }


    #region  Main Loop
    void Step()
    {
        while (LoopRooms())
        {

        }
        _debugButton = false;
        Cleanup();


    }

    bool LoopRooms()
    {
        GameObject roomObj = null;

        RoomAttachPoint point = null;
        RoomData curRoom = null;
        int iterator = 0;

        while (true)
        {
            iterator++;

            if (_totalQouta <= 0)
            {
                return false;
            }

            if (iterator > 10000)
            {
                throw new System.Exception("Loop exceted 9000 | LoopRooms");
            }
            if (_validRooms.Count <= 0)
            {
                throw new System.Exception("No valid rooms found while qouta has not been meet");
            }


            curRoom = PickRandomValidRoom();



            if (!TryFindValidPrefabAndAttach(curRoom, out roomObj, out point))
            {
                _validRooms.Remove(curRoom);
                continue;
            }
            break;


        }

        TryAddRoom(curRoom, roomObj, point);
        return true;
    }
    #endregion

    #region Room Generation
    void TryAddRoom(RoomData curRoom, GameObject roomObj, RoomAttachPoint point)
    {
        _roomsBuild++;
        GameObject iRoomObj = Instantiate(roomObj);

        RoomData iRoomData = iRoomObj.GetComponent<RoomData>();

        RoomAttachPoint otherPoint = iRoomData.PickRandomValidAttachPoint(point, iRoomData);

        Vector3 ConnectedOfset = otherPoint.transform.position - iRoomObj.transform.position;
        Vector3 AttachPointPos = point.transform.position;

        Vector3 finalPos = AttachPointPos - ConnectedOfset;

        iRoomObj.transform.position = finalPos;

        iRoomObj.name += _roomsBuild;



        if (iRoomData.BoundingBox == null)
        {
            //print("Colider was null");
            iRoomData.BoundingBox = iRoomData.GetComponent<BoxCollider2D>();
        }
        //!Physics2D.IsTouching(iRoomData.BoundingBox, curRoom.BoundingBox)
        Physics2D.SyncTransforms();

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            iRoomData.transform.position,
            iRoomData.BoundingBox.bounds.size,
            0f
        );
        bool hasHitSomething = false;

        foreach (var hit in hits)
        {
            //print(hit.name);
            if (hit != curRoom.BoundingBox && hit != iRoomData.BoundingBox)
            {
                hasHitSomething = true;
                break;
            }
        }

        if (hasHitSomething)
        {

            //print($"Something Was Touching {point.transform.parent.name} | {point.transform.name}");
            Destroy(iRoomObj);

        }
        else
        {
            _validRooms.Add(iRoomData);
            iRoomData.ValidRoomAttachPoints.Remove(otherPoint);
            iRoomData.CurAttachments++;
            iRoomData.ConnectedPoint = point;

            curRoom.CurAttachments++;
            _curRooms.Add(iRoomData);
            point.Connected = iRoomData;
        }



        if (!(_quotas[iRoomData.RoomType] == -1))
        {
            _quotas[iRoomData.RoomType]--;
            _totalQouta--;
        }



        curRoom.ValidRoomAttachPoints.Remove(point);




    }

    #endregion

    #region Validation
    bool TryFindValidPrefabAndAttach(RoomData roomData, out GameObject roomObj, out RoomAttachPoint curPoint)
    {
        roomObj = null;
        curPoint = null;
        /* 
        Loops over every attach point and checks if it has any valid prefabs, if not it is removed from the pool
        and another attach point is choosen. This repeats until either a valid prefab is found or every point is removed
        at which point it returns
        */
        int iterator = 0;
        while (true)
        {
            iterator++;

            if (iterator > 10000)
            {
                throw new System.Exception("Loop exceted 9000 | TryFindValidPrefabAndAttach");
            }
            //Guard Statements
            if (roomData.ValidRoomAttachPoints.Count <= 0)
            {
                //Debug.Log("Nothing Valid Found");
                return false;
            }
            if (roomData.CurAttachments >= roomData.MaxAttachments)
            {
                //Debug.Log("To many attachments :(");
                return false;
            }


            curPoint = roomData.GetRandomAttachPoint();

            if (TryFindRandomValidAttachRoom(out roomObj, curPoint))
            {
                break;
            }
            else
            {
                roomData.ValidRoomAttachPoints.Remove(curPoint);
                curPoint = null;
            }
        }
        return true;
    }





    bool TryFindRandomValidAttachRoom(out GameObject obj, RoomAttachPoint point)
    {

        obj = null;
        //This is so stupid and i hate lists
        List<WeightedObject> validRoomPrefabs = _genData.Objects.ToList();
        List<WeightedObject> RoomPrefabs = _genData.Objects.ToList();

        // Loops over rooms and removes invalid rooms
        for (int i = 0; i < RoomPrefabs.Count; i++)
        {
            var room = RoomPrefabs[i].Object;
            RoomData roomData = room.GetComponent<RoomData>();

            if (!IsRoomValid(roomData, point))
            {
                validRoomPrefabs.Remove(RoomPrefabs[i]);
                continue;
            }

        }

        if (validRoomPrefabs.Count <= 0)
        {
            //Debug.Log("No valid rooms found");
            return false;
        }



        int TotalWeight = 0;
        foreach (var room in validRoomPrefabs)
        {
            TotalWeight += room.Weight;
            //print($"{room.Object.name} | {Time.time}");
        }

        //print($"total weight {TotalWeight} | {Time.time}");

        int rndWeight = GenRnd.Next(TotalWeight);

        //print($"rnd weight {rndWeight} | {Time.time}");

        foreach (var room in validRoomPrefabs)
        {
            rndWeight -= room.Weight;
            if (rndWeight <= 0)
            {
                obj = room.Object;
                break;
            }
        }

        //int index = GenRnd.Next(weightedRoomPrefabs.Count);
        //obj = weightedRoomPrefabs[index];

        return true;
    }

    bool IsRoomValid(RoomData roomData, RoomAttachPoint point)
    {
        if (!point.ValidRoomTypes.Contains(roomData.RoomType))
        {
            //print($"Room doesn't have corect data type | {roomData.gameObject.name}");
            return false;
        }


        bool hasValidAttachPoint = false;
        foreach (var roomPrefabPoint in roomData.RoomAttachPoints)
        {
            if (roomPrefabPoint.ConectingDir == (int)point.Direction)
            {
                hasValidAttachPoint = true;
            }
        }
        if (!hasValidAttachPoint)
        {
            //print($"Room doesn't have point with valid direction | {roomData.gameObject.name}");
            return false;
        }


        //Checks if there is still qouta left
        if (_quotas[roomData.RoomType] == -1)
        {
            return true;
        }
        if (_quotas[roomData.RoomType] <= 0)
        {
            return false;
        }

        return true;

    #endregion
    }


    #region  Random
    RoomData PickRandomValidRoom()
    {
        int index = GenRnd.Next(_validRooms.Count);
        return _validRooms[index];

    }
    #endregion

    #region Traversal


    void FindFarthestRoom(RoomData roomData)
    {
        List<(RoomData, int)> ToTraverse = new();
        List<(RoomData, int)> TraversedItems = new();
        ToTraverse.Add((roomData, 0));

        while (ToTraverse.Count > 0)
        {
            RoomData curRoom = ToTraverse[0].Item1;
            TraversedItems.Add((curRoom, ToTraverse[0].Item2));
            foreach (RoomAttachPoint point in curRoom.RoomAttachPoints)
            {
                if (point.Connected != null)
                {
                    ToTraverse.Add((point.Connected, ToTraverse[0].Item2 + 1));
                }
            }
        }
    }

    #endregion

    #region Cleanup



    void Cleanup()
    {
        List<RoomData> ToDelete = new();
        foreach (RoomData room in _curRooms)
        {
            //print(room.RoomType);

            if (_genData.CleanUpTypes.Contains(room.RoomType))
            {
                if (room.CurAttachments != room.MaxAttachments)
                {
                    ToDelete.Add(room);
                }
            }
        }

        foreach (RoomData room in ToDelete)
        {
            room.ConnectedPoint.Connected = null;
            _curRooms.Remove(room);
            Destroy(room.gameObject);
        }
    }

    #endregion

}

