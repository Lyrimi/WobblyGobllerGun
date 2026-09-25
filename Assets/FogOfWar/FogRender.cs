using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static FogManager;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Android.Gradle.Manifest;
using System.Linq;
using System;
using System.Drawing;

public class FogRender : MonoBehaviour
{

    [SerializeField] private Camera _mainCam;
    [SerializeField] private ComputeShader _fogShader;
    [SerializeField] private RawImage displayImage;
    private int _shaderKernel;
    private Transform _camTransform;
    private FogManager _fogManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    const int CHUNKVIEW_HEIGHT = 2;
    const int CHUNKVIEW_WIDTH = 3;

    private RenderTexture _fogRendTex;
    ComputeBuffer _fogDataBuffer;

    private int[] _curFogData;

    int _textHeight;
    int _textWidth;


    void Start()
    {
        _fogManager = FogManager.instance;
        _camTransform = _mainCam.transform;


        _shaderKernel = _fogShader.FindKernel("CSMain");

        InitTex();

        _curFogData = new int[_textHeight * _textWidth];
        _fogDataBuffer = new(_curFogData.Length, sizeof(int));
    }

    void InitTex()
    {
        _textHeight = CHUNKVIEW_HEIGHT * CHUNK_SIZE;
        _textWidth = CHUNKVIEW_WIDTH * CHUNK_SIZE;




        _fogRendTex = new(_textWidth, _textHeight, 0, RenderTextureFormat.ARGB32)
        {
            enableRandomWrite = true,
            filterMode = FilterMode.Bilinear,
        };
        _fogRendTex.Create();

        displayImage.texture = _fogRendTex;

    }


    void SyncFogTexture()
    {
        //Updates the current fog data in view

        int[][,] chunks = _fogManager.GetFogEntries(FindActiveChunks());

        _curFogData = MergeData(chunks);

        _fogDataBuffer.SetData(_curFogData);

        //Sets and runs the shader
        _fogShader.SetInt("Width", _textWidth);
        _fogShader.SetInt("Height", _textHeight);
        _fogShader.SetTexture(_shaderKernel, "Result", _fogRendTex);
        _fogShader.SetBuffer(_shaderKernel, "FogData", _fogDataBuffer);

        _fogShader.Dispatch(_shaderKernel, Mathf.CeilToInt(_textWidth / 8), Mathf.CeilToInt(_textHeight / 8), 1);
    }

    int[] MergeData(int[][,] data)
    {
        //asumes every data is same length
        int count = data.Length;
        int dataLen = data[0].Length;

        int[] flatData = new int[count * dataLen];

        int byteCopySize = sizeof(int) * data[0].Length;

        int byteIndex = 0;
        foreach (var src in data)
        {
            Buffer.BlockCopy(src, 0, flatData, byteIndex, byteCopySize);
            byteIndex += byteCopySize;
        }
        return flatData;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Vector2Int cCord in FindActiveChunks())
        {
            if (!_fogManager.FogData.ContainsKey(cCord))
            {
                _fogManager.CreateFogEntry(cCord);
            }

            _fogManager.DebugDisplayChunk(cCord, new(0, 1f, 0), 0.1f);

            SyncFogTexture();
        }
    }

    Vector2Int[] FindActiveChunks()
    {
        float zPos = _mainCam.nearClipPlane;
        Vector3 conor = _mainCam.ViewportToWorldPoint(new(0, 0, zPos));

        Vector2Int CorChunkPos = GlobalToChunk(conor);

        Vector2Int[] activeChunks = new Vector2Int[CHUNKVIEW_HEIGHT * CHUNKVIEW_WIDTH];

        for (int y = 0; y < CHUNKVIEW_HEIGHT; y++)
        {
            for (int x = 0; x < CHUNKVIEW_WIDTH; x++)
            {
                activeChunks[y * CHUNKVIEW_WIDTH + x] = CorChunkPos + new Vector2Int(x, y);
            }
        }

        return activeChunks;

    }
}
