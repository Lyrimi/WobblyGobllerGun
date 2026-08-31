using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using static RoomTypes;

public class RoomData : MonoBehaviour
{
    public int MaxAttachments;
    [NonSerialized]
    public int CurAttachments = 0;

    public RoomType RoomType;

    public RoomAttachPoint[] RoomAttachPoints;
    public List<RoomAttachPoint> ValidRoomAttachPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ValidRoomAttachPoints = RoomAttachPoints.ToList();
    }

    public RoomAttachPoint GetRandomAttachPoint(Random rnd)
    {
        int index = rnd.Next(ValidRoomAttachPoints.Count);
        return ValidRoomAttachPoints[index];
    }
}
