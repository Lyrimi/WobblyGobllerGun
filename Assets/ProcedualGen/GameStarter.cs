using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(RoomData))]
public class GameStarter : MonoBehaviour
{
    public GameObject PlayerObj;
    private RoomData _roomData;
    [SerializeField] private GameObject _spawnPos;
    private Tilemap _wallTilemap;
    //This function is called when the generation script is done with all it's funky stuff
    void Start()
    {

    }
    public void StartGame()
    {
        _wallTilemap = Generation.Instance.GlobalWallTileMap;
        _roomData = GetComponent<RoomData>();
        PlayerObj.SetActive(true);

        Vector3 pos = _spawnPos.transform.position;

        pos.z = PlayerObj.transform.position.z;

        PlayerObj.transform.position = pos;

        //Vector2 TmapPos = _wallTilemap.CellToWorld(_wallTilemap.WorldToCell(_spawnPos.transform.position));
        //StartCoroutine(_roomData.RevealTiles(TmapPos));
    }
    // Update is called once per frame
    void Update()
    {

    }
}
