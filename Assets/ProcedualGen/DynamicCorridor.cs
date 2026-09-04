using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicCorridor : MonoBehaviour
{
    [SerializeField] private TileBase _groundTile;

    [SerializeField] private TileBase _wallTile;

    [SerializeField] private Tilemap _groundTilemap;

    [SerializeField] private Tilemap _wallTilemap;

    [Header("Debug")]


    [SerializeField] private int _length;

    [SerializeField] private RoomAttachPoint _roomAttachPoint;

    [SerializeField] private bool _debugButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_debugButton)
        {
            _debugButton = false;
            GenCorridor(_roomAttachPoint, _length);
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
}
