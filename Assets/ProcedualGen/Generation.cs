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

    private GenData _genData;
    private List<RoomData> _validRooms;


    [SerializeField]
    private bool _debugButton;

    [SerializeField]
    private bool _debugButton2;

    [SerializeField]
    private ContactFilter2D _filter;

    public static System.Random GenRnd;

    private int _roomsBuild = 0;

    private Dictionary<RoomType, int> _quotas;

    [SerializeField]

    private List<RoomData> _curRooms;

    [SerializeField]
    private bool _debugTraversel;

    [SerializeField]
    private int _maxIterations = 10000;

    private int _totalQouta;

    private int _stepBackAmount = 1;
    private int _curBuildStreak = 0;

    private int _lastBuildStrak = 0;


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
            RoomData StartRoom = FindFarthestRoom(_curRooms[0]);
            RoomData EndRoom = FindFarthestRoom(StartRoom);

            StartRoom.GetComponent<SpriteRenderer>().color = new(0, 1, 0);
            EndRoom.GetComponent<SpriteRenderer>().color = new(0, 0, 1);

        }
    }


    #region  Main Loop
    void Step()
    {
        if (LoopRooms())
        {
            return;
        }
        Cleanup();
        RoomData StartRoom = FindFarthestRoom(_curRooms[0]);
        RoomData EndRoom = FindFarthestRoom(StartRoom);

        StartRoom.GetComponent<SpriteRenderer>().color = new(0, 1, 0);
        EndRoom.GetComponent<SpriteRenderer>().color = new(0, 0, 1);



    }

    bool LoopRooms()
    {
        GameObject roomObj = null;

        RoomAttachPoint point = null;
        RoomData curRoom = null;
        int iterator = 0;
        int problemCount = 0;

        while (true)
        {
            iterator++;

            if (_totalQouta <= 0)
            {
                return false;
            }

            if (iterator > _maxIterations)
            {
                throw new System.Exception("Loop exceted 9000 | LoopRooms");
            }
            if (_validRooms.Count <= 0)
            {
                //
                Debug.Log("Problems");
                if (problemCount >= 100)
                {

                    throw new System.Exception("No valid rooms found while qouta has not been meet");
                }
                problemCount++;
                StepBack();

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
            otherPoint.Connected = curRoom;

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

        _curBuildStreak++;




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

            if (iterator > _maxIterations)
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


    RoomData FindFarthestRoom(RoomData roomData)
    {
        List<(RoomData, int)> ToTraverse = new();
        List<(RoomData, int)> TraversedItems = new();
        List<RoomData> TraversedRooms = new();
        ToTraverse.Add((roomData, 0));
        int _debugFuthersDistance = 0;

        int iterator = 0;
        while (ToTraverse.Count > 0)
        {
            iterator++;
            if (iterator > _maxIterations)
            {
                throw new System.Exception("Loop exceted 9000 | LoopRooms");
            }


            RoomData curRoom = ToTraverse[0].Item1;
            int depth = ToTraverse[0].Item2;
            TraversedItems.Add((curRoom, ToTraverse[0].Item2));
            TraversedRooms.Add(curRoom);

            if (depth > _debugFuthersDistance)
            {
                _debugFuthersDistance = depth;
            }

            ToTraverse.RemoveAt(0);



            foreach (RoomAttachPoint point in curRoom.RoomAttachPoints)
            {
                if (point.Connected != null)
                {
                    if (TraversedRooms.Contains(point.Connected))
                    {
                        continue;
                    }
                    ToTraverse.Add((point.Connected, depth + 1));

                }
            }
        }
        RoomData furthestRoom = null;
        int distance = 0;
        foreach (var item in TraversedItems)
        {
            if (_debugTraversel)
            {
                item.Item1.GetComponent<SpriteRenderer>().color = new((float)item.Item2 / (float)_debugFuthersDistance, 0, 0);
            }
            if (item.Item2 > distance)
            {
                distance = item.Item2;
                furthestRoom = item.Item1;
            }
        }

        return furthestRoom;
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
            room.ConnectedPoint.Parrent.CurAttachments--;
            room.ConnectedPoint.Connected = null;

            _curRooms.Remove(room);
            Destroy(room.gameObject);
        }
    }

    void StepBack()
    {
        //Recaculates how much to step back to increase build speed
        if (_lastBuildStrak - 10 < _curBuildStreak && _curBuildStreak < _lastBuildStrak + 10)
        {
            _stepBackAmount++;
            print($"Addedd: {_stepBackAmount}");
        }
        else
        {
            _stepBackAmount = 1;
            print($" Reset{_stepBackAmount}");
        }

        _lastBuildStrak = _curBuildStreak;
        _curBuildStreak = 0;

        int count = _curRooms.Count;
        RoomData lastRemovedRoom = null;
        List<RoomData> roomsToRemove = new();
        for (int i = 0; i < _stepBackAmount; i++)
        {
            count = _curRooms.Count;
            RoomData removedRoom = _curRooms.Last();
            if (removedRoom == _curRooms[0])
            {
                break;
            }



            _curRooms.RemoveAt(count - 1);

            removedRoom.ConnectedPoint.Connected = null;
            removedRoom.ConnectedPoint.Parrent.CurAttachments--;
            removedRoom.ConnectedPoint.Parrent.ValidRoomAttachPoints.Remove(removedRoom.ConnectedPoint);
            //Adds the qoutas for the removed objects
            if (!(_quotas[removedRoom.RoomType] == -1))
            {
                _totalQouta++;
                _quotas[removedRoom.RoomType]++;
            }

            lastRemovedRoom = removedRoom;

            roomsToRemove.Add(removedRoom);

        }

        foreach (var removedRoom in roomsToRemove)
        {
            Destroy(removedRoom.gameObject);
            Destroy(removedRoom);
        }

        _validRooms.Add(lastRemovedRoom.ConnectedPoint.Parrent);

        count = _curRooms.Count;
        for (int i = 0; i < 4; i++)
        {
            if ((count - i) > -1)
            {
                RoomData room = _curRooms[count - i - 1];
                if (_validRooms.Contains(room))
                {
                    _validRooms.Add(room);
                }
            }
        }






    }

    #endregion

}

