using System.Collections.Generic;
using UnityEngine;

public class FogManager : MonoBehaviour
{
    public Dictionary<Vector2Int, byte[,]> FogData = new();
    public const int CHUNK_SIZE = 16;

    public static FogManager instance;

    [Header("Deubg")]
    [SerializeField] bool _debugDisplayChunks;
    [SerializeField] Vector2Int _chunkCord;
    [SerializeField] bool _debugWriteChunkData;
    [SerializeField] bool _debugDisplayOnWrite;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_debugDisplayChunks)
        {
            _debugDisplayChunks = false;
            foreach (var item in FogData)
            {
                DebugDisplayChunk(item.Key, new(0,0,1f), 1000);
            }
        }

        if (_debugWriteChunkData)
        {
            _debugWriteChunkData = false;
            CreateFogEntry(_chunkCord);
        }

        
    }

    public static Vector2Int GlobalToChunk(Vector2 GlobalPos)
    {
        int cx = Mathf.RoundToInt(GlobalPos.x / CHUNK_SIZE);
        int cy = Mathf.RoundToInt(GlobalPos.y / CHUNK_SIZE);

        return new(cx,cy);
    }

    public void CreateFogEntry(Vector2Int cCord)
    {
        if (FogData.ContainsKey(cCord))
        {
            return;
        }

        //Creates new chunk data with everything set to zero
        byte[,] newChunkData = new byte[CHUNK_SIZE, CHUNK_SIZE];
        FogData.Add(cCord, newChunkData);
        if (_debugDisplayOnWrite)
        {
            DebugDisplayChunk(cCord, new(1f,0,0), 1000);
        }
    }

    public void DebugDisplayChunk(Vector2Int cCord, Color color, float duration)
    {
        Vector2 globalPos = cCord * CHUNK_SIZE;
        DrawDebugSquare(globalPos, CHUNK_SIZE, color, duration);
    }
    
    static void DrawDebugSquare(Vector3 center, float size, Color color, float duration = 0f)
    {
        float half = size / 2.0f;

        // Calculate the 4 corner positions (Assuming flat on the X/Y plane)
        Vector3 topLeft     = center + new Vector3(-half,  half, 0f);
        Vector3 topRight    = center + new Vector3( half,  half, 0f);
        Vector3 bottomRight = center + new Vector3( half, -half, 0f);
        Vector3 bottomLeft  = center + new Vector3(-half, -half, 0f);

        // Draw the 4 bounding lines connecting the corners
        Debug.DrawLine(topLeft, topRight, color, duration);
        Debug.DrawLine(topRight, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration);
        Debug.DrawLine(bottomLeft, topLeft, color, duration);
    }
}
