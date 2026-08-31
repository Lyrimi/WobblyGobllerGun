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
    void Start()
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
        while (true)
        {
            RoomData curRoom = PickRandomValidRoom();
            if (curRoom.ValidRoomAttachPoints.Count <= 0)
            {
                break;
            }
            RoomAttachPoint curPoint = curRoom.GetRandomAttachPoint(_rnd);
            GameObject roomObj;
            if (FindValidAttachRoom(out roomObj, curPoint))
            {
                break;
            }
            else
            {
                curRoom.ValidRoomAttachPoints.Remove(curPoint);
            }

        }

    }



    RoomData PickRandomValidRoom()
    {
        int index = _rnd.Next(_validRooms.Count);
        return _validRooms[index];
    }

    bool FindValidAttachRoom(out GameObject obj, RoomAttachPoint point)
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
                validRoomPrefabs.RemoveAt(i);
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
            return false;
        }
        return true;
    }


}
