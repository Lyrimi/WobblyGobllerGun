using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static FogManager;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Android.Gradle.Manifest;
using System.Linq;
using System;
using System.Drawing;
using UnityEditor.U2D.Aseprite;

public class FogRender : MonoBehaviour
{

    [SerializeField] private Camera _mainCam;
    [SerializeField] private ComputeShader _fogShader;
    [SerializeField] private RawImage displayImage;
    [SerializeField] private GameObject _rendCanvas;
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
        Vector2Int[] activeChunks = FindActiveChunks();
        int[][,] chunks = _fogManager.GetFogEntries(activeChunks);

        _curFogData = MergeData(chunks);

        _fogDataBuffer.SetData(_curFogData);

        //Sets and runs the shader
        _fogShader.SetInt("Width", _textWidth);
        _fogShader.SetInt("Height", _textHeight);
        _fogShader.SetInt("CHUNKSIZE", CHUNK_SIZE);
        _fogShader.SetInt("Cw", CHUNKVIEW_WIDTH);
        _fogShader.SetInt("Ch", CHUNKVIEW_HEIGHT);


        _fogShader.SetTexture(_shaderKernel, "Result", _fogRendTex);
        _fogShader.SetBuffer(_shaderKernel, "FogData", _fogDataBuffer);

        _fogShader.Dispatch(_shaderKernel, Mathf.CeilToInt(_textWidth / 8), Mathf.CeilToInt(_textHeight / 8), 1);

        MoveCanvas(activeChunks[0]);
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
    void LateUpdate()
    {
        foreach (Vector2Int cCord in FindActiveChunks())
        {
            if (!_fogManager.FogData.ContainsKey(cCord))
            {
                _fogManager.CreateFogEntry(cCord);
            }

            _fogManager.DebugDisplayChunk(cCord, new(0, 1f, 0), 0.1f);

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    Vector2 dis = new Vector2(x, y) + new Vector2(0.5f, 0.5f);
                    _fogManager.SetPixelData(_camTransform.position + (Vector3)dis, 1);
                }
            }

            SyncFogTexture();
        }
    }



    void MoveCanvas(Vector2Int cChunk)
    {

        float tx = (CHUNKVIEW_WIDTH * CHUNK_SIZE) / 2;
        float ty = (CHUNKVIEW_HEIGHT - 2 * CHUNK_SIZE) / 2;
        Vector2 lCenter = new(tx, -ty);

        Vector2 chunkGpos = cChunk * CHUNK_SIZE;

        Vector2 globalPos = chunkGpos + lCenter;

        _rendCanvas.transform.position = globalPos;
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
