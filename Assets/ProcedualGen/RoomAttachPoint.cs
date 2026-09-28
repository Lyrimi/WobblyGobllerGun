using System;
using System.Collections.Generic;
using UnityEngine;
using static RoomTypes;

public class RoomAttachPoint : MonoBehaviour
{
    public List<RoomType> ValidRoomTypes;
    public Direction Direction;

    public uint MinCorridorLength;
    public uint MaxCorridorLength;

    [NonSerialized]
    public RoomData Parrent;
    [NonSerialized]
    public RoomData Connected;

    public int ConectingDir
    {
        get
        {
            return ((int)Direction + 180) % 360;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public Vector2 GetVectorDirection()
    {
        Vector2 direction = new();
        switch (Direction)
        {
            case Direction.North:
                direction = new(0, 1);
                break;
            case Direction.East:
                direction = new(1, 0);
                break;
            case Direction.South:
                direction = new(0, -1);
                break;
            case Direction.West:
                direction = new(-1, 0);
                break;
            default:
                break;
        }

        return direction;
    }
}
