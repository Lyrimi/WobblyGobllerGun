using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Generation : MonoBehaviour
{
    [SerializeField]
    GenData _genData;
    [SerializeField]
    List<RoomData> _validRooms;


    [SerializeField]
    bool _debugButton;

    static System.Random _rnd;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (_rnd == null)
        {
            _rnd = new();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (_debugButton)
        {
            _debugButton = false;
            Step();
        }
    }

    void Step()
    {
        if (_validRooms.Count <= 0)
        {
            Debug.Log("Out of Rooms");
            return;

        }
        GameObject roomObj = null;

        RoomData curRoom = PickRandomValidRoom();
        while (true)
        {
            if (curRoom.ValidRoomAttachPoints.Count <= 0)
            {
                Debug.Log("Nothing Valid Found");
                return;
            }
            if (curRoom.CurAttachments >= curRoom.MaxAttachments)
            {
                Debug.Log("To many attachments :(");
                return;
            }
            RoomAttachPoint curPoint = curRoom.GetRandomAttachPoint(_rnd);

            if (FindRandomValidAttachRoom(out roomObj, curPoint))
            {
                break;
            }
            else
            {
                curRoom.ValidRoomAttachPoints.Remove(curPoint);
            }

        }

        curRoom.CurAttachments++;
        Instantiate(roomObj);

    }



    RoomData PickRandomValidRoom()
    {
        int index = _rnd.Next(_validRooms.Count);
        return _validRooms[index];
    }

    bool FindRandomValidAttachRoom(out GameObject obj, RoomAttachPoint point)
    {

        obj = null;
        List<GameObject> validRoomPrefabs = _genData.Objects.ToList();
        List<GameObject> RoomPrefabs = _genData.Objects.ToList();
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
            Debug.Log("No valid rooms found");
            return false;
        }

        int index = _rnd.Next(validRoomPrefabs.Count);
        obj = validRoomPrefabs[index];

        return true;
    }

    bool IsRoomValid(RoomData roomData, RoomAttachPoint point)
    {
        if (!point.ValidRoomTypes.Contains(roomData.RoomType))
        {
            print($"Room doesn't have corect data type | {roomData.gameObject.name}");
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
            print($"Room doesn't have point with valid direction | {roomData.gameObject.name}");
            return false;
        }
        return true;
    }


}
