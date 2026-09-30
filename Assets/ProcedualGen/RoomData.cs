using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using static RoomTypes;

using System.Collections;
using Unity.Mathematics;
using UnityEngine.Tilemaps;
using static FogManager;
using Unity.VisualScripting;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(RoomDirector))]
public class RoomData : MonoBehaviour
{

    public int MaxAttachments;

    public int CurAttachments = 0;

    public string Identifyer;

    public RoomType RoomType;

    public RoomAttachPoint[] RoomAttachPoints;
    public List<RoomAttachPoint> ValidRoomAttachPoints;

    public List<RoomAttachPoint> ConnectedPoints;

    [Header("Spawning")]
    public List<GameObject> EnemySpawnPoints;
    public int MaxEnemySpawnPrWave;

    public float MinDistanceToSpawn;


    [NonSerialized]
    public List<(RoomAttachPoint, DynamicCorridor)> AttachedCorridors = new();

    [NonSerialized]
    public BoxCollider2D BoundingBox;

    [NonSerialized]
    public Generation Generation;

    [NonSerialized]
    public RoomDirector RoomDirector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        print(RoomAttachPoints.Length + $" list | {gameObject.name}");
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



    void RevealTile(Vector2 pos)
    {
        //Ofset because i DON'T FUCKING KNOW WHY IT HATES ME
        FogManager.instance.SetPixelData(pos + new Vector2(-0.5f, 0.5f), 1);
    }

    [SerializeField] private float _stepTime = 0.05f;
    [SerializeField] private float _stepTimeProcent = 0.01f;
    public IEnumerator RevealTiles(Vector2 startPos, Action callBackFinish = null)
    {
        Tilemap walls = Generation.Instance.GlobalWallTileMap;
        Tilemap ground = Generation.Instance.GlobalGroundTileMap;
        Dictionary<Vector2, int> traversedItems = new();
        List<Vector2> toTraverse = new()
        {
            startPos
        };
        List<Vector2> toTraveseNext = new();


        void TryExpand(Vector2 pos, Vector2 ofset)
        {
            Vector2 nCord = pos + ofset;

            //these checks are going to fucking kill me
            if (traversedItems.ContainsKey(nCord))
            {

                return;
            }
            if (!BoundingBox.OverlapPoint(nCord))
            {
                if (FogManager.instance.DebugDisplayOnWrite)
                {
                    DrawDebugSquare(pos, 0.5f, Color.darkGreen, 10000);
                }

                return;
            }

            Vector3Int twPos = walls.WorldToCell(nCord);

            if (ground.GetTile(twPos) == null)
            {
                if (FogManager.instance.DebugDisplayOnWrite)
                {
                    DrawDebugSquare(pos, 0.5f, Color.red, 10000);
                }
                return;
            }

            traversedItems.Add(nCord, 1);
            toTraveseNext.Add(nCord);
            RevealTile(nCord);
        }

        while (toTraverse.Count > 0)
        {
            foreach (Vector2 pos in toTraverse)
            {
                Vector3Int twPos = walls.WorldToCell(pos);
                if (walls.GetTile(twPos) != null)
                {
                    if (FogManager.instance.DebugDisplayOnWrite)
                    {
                        DrawDebugSquare(pos, 0.5f, Color.pink, 10000);
                    }
                    continue;
                }
                TryExpand(pos, new(0, 1));
                TryExpand(pos, new(0, -1));
                TryExpand(pos, new(1, 0));
                TryExpand(pos, new(-1, 0));
            }
            toTraverse = toTraveseNext;
            toTraveseNext = new();

            yield return new WaitForSeconds(_stepTime);
            _stepTimeProcent *= 1.2f;
            _stepTime -= _stepTime * _stepTimeProcent;
        }

        callBackFinish();
    }


}
