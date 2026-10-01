using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using static FogManager;
[RequireComponent(typeof(BoxCollider2D))]

public class DynamicCorridor : MonoBehaviour
{
    [SerializeField] private TileBase _groundTile;

    [SerializeField] private TileBase _wallTile;

    [SerializeField] private Tilemap _groundTilemap;

    [SerializeField] private Tilemap _wallTilemap;

    [Header("Debug")]

    [SerializeField] private Vector2 _debugAttachPointCords;


    [SerializeField] private int _length;

    [SerializeField] private RoomAttachPoint _roomAttachPoint;

    [SerializeField] private bool _debugButton;
    [SerializeField] private bool _debugButton2;
    [SerializeField] private Tilemap wallOveride;
    private BoxCollider2D _boundingBox;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _boundingBox = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_debugButton)
        {
            _debugButton = false;
            GenCorridor(_roomAttachPoint, _length);
        }
        if (_debugButton2)
        {
            _debugButton2 = false;
            StartCoroutine(RevealTiles(_debugAttachPointCords));
        }
    }

    public void GenCorridor(RoomAttachPoint point, int len)
    {
        // If corrddior length is zero or less it kills it self
        if (len <= 0)
        {
            Destroy(gameObject);
            Destroy(this);
        }
        Vector2 dir = point.GetVectorDirection();
        Vector2Int intDir = new Vector2Int(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y));

        Vector3Int curPoint = new(0, 0, 0);

        for (int i = 0; i < len; i++)
        {
            GenStep(curPoint, intDir);
            curPoint += (Vector3Int)intDir;
        }
        AttachCorridor(point);
        BuildBoundingBox(intDir, len);
    }

    void GenStep(Vector3Int point, Vector2Int dir)
    {
        Vector2Int perDir = new Vector2Int(-dir.y, dir.x);

        Vector3Int[] points = new Vector3Int[3];

        points[0] = point;
        points[1] = point + (Vector3Int)perDir;
        points[2] = point - (Vector3Int)perDir;

        foreach (Vector3Int setPoint in points)
        {
            if (setPoint != point)
            {
                _wallTilemap.SetTile(setPoint, _wallTile);
            }
            _groundTilemap.SetTile(setPoint, _groundTile);
        }
    }

    void BuildBoundingBox(Vector2Int dir, int length)
    {

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        print(dir);
        Vector2Int perDir = new Vector2Int(-dir.y, dir.x);
        Vector2Int posDir = new Vector2Int(math.abs(dir.x), math.abs(dir.y));

        Vector2 size = posDir * length;
        size.x += math.abs(perDir.x * 3);
        size.y += math.abs(perDir.y * 3);

        box.size = size;

        Vector2 roomLen = dir * length;
        Vector2 center = new((roomLen.x - dir.x) / 2 * posDir.x, (roomLen.y - dir.y) / 2 * posDir.y);

        box.offset = center;
    }

    void AttachCorridor(RoomAttachPoint point)
    {
        //This is so easy because the origin point of the corridor is the attach point :)
        transform.position = point.transform.position;
    }

    void RevealTile(Vector2 pos)
    {
        //Ofset because i DON'T FUCKING KNOW WHY IT HATES ME
        FogManager.instance.SetPixelData(pos + new Vector2(0, 0.5f), 1);
    }

    [SerializeField] private float _stepTime = 0.1f;
    public IEnumerator RevealTiles(Vector2 startPos, RoomData room = null)
    {
        if (room != null && room._isEvilAssRoom)
        {
            yield break;
        }
        Tilemap walls = Generation.Instance.GlobalWallTileMap;
        Tilemap ground = Generation.Instance.GlobalGroundTileMap;

        startPos = walls.GetCellCenterWorld(walls.WorldToCell(startPos));

        if (wallOveride != null)
        {
            walls = wallOveride;
        }
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
            if (!_boundingBox.OverlapPoint(nCord))
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
        }


    }
}
