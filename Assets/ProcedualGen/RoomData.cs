using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using static RoomTypes;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(RoomDirector))]
public class RoomData : MonoBehaviour
{

    public int MaxAttachments;

    public int CurAttachments = 0;

    public RoomType RoomType;

    public RoomAttachPoint[] RoomAttachPoints;
    public List<RoomAttachPoint> ValidRoomAttachPoints;

    public List<RoomAttachPoint> ConnectedPoints;

    public List<GameObject> EnemySpawnPoints;

    [NonSerialized]
    public BoxCollider2D BoundingBox;

    [NonSerialized]
    public Generation Generation;

    [NonSerialized]
    public RoomDirector RoomDirector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        ValidRoomAttachPoints = RoomAttachPoints.ToList();
        BoundingBox = GetComponent<BoxCollider2D>();
        RoomDirector = GetComponent<RoomDirector>();
        foreach (var point in RoomAttachPoints)
        {
            point.Parrent = this;
        }
    }

    public RoomAttachPoint GetRandomAttachPoint()
    {
        Random rnd = Generation.GenRnd;
        int index = rnd.Next(ValidRoomAttachPoints.Count);
        return ValidRoomAttachPoints[index];
    }

    public RoomAttachPoint PickRandomValidAttachPoint(RoomAttachPoint basePoint, RoomData PrefabRoom)
    {
        Random rnd = Generation.GenRnd;
        List<RoomAttachPoint> curValidPoints = RoomAttachPoints.ToList();
        foreach (var point in RoomAttachPoints)
        {
            if (!(basePoint.ConectingDir == (int)point.Direction))
            {
                curValidPoints.Remove(point);
            }
        }

        if (curValidPoints.Count <= 0)
        {
            return null;
        }

        int index = rnd.Next(curValidPoints.Count);
        return curValidPoints[index];



    }
}
