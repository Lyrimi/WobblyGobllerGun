using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FogManager : MonoBehaviour
{
    public Dictionary<Vector2Int, int[,]> FogData = new();
    public const int CHUNK_SIZE = 16;

    public static FogManager instance;

    [Header("Deubg")]
    [SerializeField] bool _debugDisplayChunks;
    [SerializeField] Vector2Int _pixCord;
    [SerializeField] bool _debugWritePixData;
    [SerializeField] public bool DebugDisplayOnWrite;

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
        FogData = new();
    }

    // Update is called once per frame
    void Update()
    {
        if (_debugDisplayChunks)
        {
            _debugDisplayChunks = false;
            foreach (var item in FogData)
            {
                DebugDisplayChunk(item.Key, new(0, 0, 1f), 1000);
            }
        }

        if (_debugWritePixData)
        {
            _debugWritePixData = false;
            SetPixelData(_pixCord, 1);

        }


    }

    public static Vector2Int GlobalToChunk(Vector2 GlobalPos)
    {
        int cx = Mathf.FloorToInt(GlobalPos.x / CHUNK_SIZE);
        int cy = Mathf.FloorToInt(GlobalPos.y / CHUNK_SIZE);

        return new(cx, cy);
    }

    public void CreateFogEntry(Vector2Int cCord)
    {
        if (FogData.ContainsKey(cCord))
        {
            return;
        }

        //Creates new chunk data with everything set to zero
        int[,] newChunkData = new int[CHUNK_SIZE, CHUNK_SIZE];
        FogData.Add(cCord, newChunkData);
        if (DebugDisplayOnWrite)
        {
            DebugDisplayChunk(cCord, new(1f, 0, 0), 1000);
        }
    }

    public void SetPixelData(Vector2 pos, int data)
    {

        Vector2Int chunkCord = GlobalToChunk(pos);
        Vector2 lcPos = pos - chunkCord * CHUNK_SIZE;
        int cx = math.abs(Mathf.FloorToInt(lcPos.x));
        int cy = math.abs(Mathf.FloorToInt(lcPos.y));
        if (!FogData.ContainsKey(chunkCord))
        {
            CreateFogEntry(chunkCord);
        }

        if (DebugDisplayOnWrite && !(FogData[chunkCord][cx, cy] == data))
        {
            Vector2 gpos = chunkCord * CHUNK_SIZE + new Vector2(cx, cy) + new Vector2(0.5f, 0.5f);
            DrawDebugSquare(gpos, 1f, Color.violet, 1000);
        }
        FogData[chunkCord][cx, cy] = data;




    }

    public void DebugDisplayChunk(Vector2Int cCord, Color color, float duration)
    {
        Vector2 globalPos = cCord * CHUNK_SIZE + new Vector2(CHUNK_SIZE / 2, CHUNK_SIZE / 2);
        DrawDebugSquare(globalPos, CHUNK_SIZE, color, duration);
    }

    public int[][,] GetFogEntries(Vector2Int[] cords)
    {
        int[][,] rData = new int[cords.Length][,];

        for (int i = 0; i < cords.Length; i++)
        {
            Vector2Int curCord = cords[i];
            if (!FogData.ContainsKey(curCord))
            {
                CreateFogEntry(curCord);
            }
            rData[i] = FogData[curCord];
        }
        return rData;
    }

    public static void DrawDebugSquare(Vector3 center, float size, Color color, float duration = 0f)
    {
        float half = size / 2.0f;

        // Calculate the 4 corner positions (Assuming flat on the X/Y plane)
        Vector3 topLeft = center + new Vector3(-half, half, 0f);
        Vector3 topRight = center + new Vector3(half, half, 0f);
        Vector3 bottomRight = center + new Vector3(half, -half, 0f);
        Vector3 bottomLeft = center + new Vector3(-half, -half, 0f);

        // Draw the 4 bounding lines connecting the corners
        Debug.DrawLine(topLeft, topRight, color, duration);
        Debug.DrawLine(topRight, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration);
        Debug.DrawLine(bottomLeft, topLeft, color, duration);
    }
}
