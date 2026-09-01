using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class Generation : MonoBehaviour
{
    [SerializeField]
    GenData _genData;
    [SerializeField]
    List<RoomData> _validRooms;


    [SerializeField]
    bool _debugButton;

    public static System.Random GenRnd;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (GenRnd == null)
        {
            GenRnd = new();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_debugButton)
        {
            Step();
        }
    }


    #region  Main Loop
    void Step()
    {
        LoopRooms();

    }

    void LoopRooms()
    {
        GameObject roomObj = null;

        RoomAttachPoint point = null;
        RoomData curRoom = null;
        int iterator = 0;

        while (true)
        {
            iterator++;

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

        AddRoom(curRoom, roomObj, point);
    }
    #endregion

    #region Generation
    void AddRoom(RoomData curRoom, GameObject roomObj, RoomAttachPoint point)
    {
        GameObject iRoomObj = Instantiate(roomObj);

        RoomData iRoomData = iRoomObj.GetComponent<RoomData>();

        RoomAttachPoint otherPoint = iRoomData.PickRandomValidAttachPoint(point, iRoomData);

        Vector3 ConnectedOfset = otherPoint.transform.position - iRoomObj.transform.position;
        Vector3 AttachPointPos = point.transform.position;

        Vector3 finalPos = AttachPointPos - ConnectedOfset;

        iRoomObj.transform.position = finalPos;

        curRoom.ValidRoomAttachPoints.Remove(point);
        iRoomData.ValidRoomAttachPoints.Remove(otherPoint);
        curRoom.CurAttachments++;
        _validRooms.Add(iRoomData);
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
                Debug.Log("Nothing Valid Found");
                return false;
            }
            if (roomData.CurAttachments >= roomData.MaxAttachments)
            {
                Debug.Log("To many attachments :(");
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
        List<GameObject> validRoomPrefabs = _genData.Objects.ToList();
        List<GameObject> RoomPrefabs = _genData.Objects.ToList();

        // Loops over rooms and removes invalid rooms
        for (int i = 0; i < RoomPrefabs.Count; i++)
        {
            var room = RoomPrefabs[i];
            RoomData roomData = room.GetComponent<RoomData>();

            if (!IsRoomValid(roomData, point))
            {
                validRoomPrefabs.Remove(room);
                continue;
            }

        }

        if (validRoomPrefabs.Count <= 0)
        {
            //Debug.Log("No valid rooms found");
            return false;
        }

        int index = GenRnd.Next(validRoomPrefabs.Count);
        obj = validRoomPrefabs[index];

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
        return true;
    }
    #endregion

    #region  Random
    RoomData PickRandomValidRoom()
    {
        int index = GenRnd.Next(_validRooms.Count);
        return _validRooms[index];

    }
    #endregion


}
