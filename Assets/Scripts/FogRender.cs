using UnityEngine;
using static FogManager;

public class FogRender : MonoBehaviour
{

    [SerializeField] Camera _mainCam;
    Transform _camTransform;
    FogManager _fogManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    const int CHUNKVIEW_HEIGHT = 2;
    const int CHUNKVIEW_WIDTH = 3;

    void Start()
    {
        _fogManager = FogManager.instance;
        _camTransform = _mainCam.transform;
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
