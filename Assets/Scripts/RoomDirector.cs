using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using Random = System.Random;
using static RoomTypes;
using Unity.Mathematics;
using UnityEngine.UIElements;

[RequireComponent(typeof(RoomData))]
public class RoomDirector : MonoBehaviour
{

    private RoomData _roomData;
    private TileBase _wallTiles;
    private Tilemap _wallTilemap;

    private GameObject[] _spawnPoints;

    private GameObject _player;



    [SerializeField] public int SpawnCredits = 0;

    private List<GameObject> _enemiesToSpawn = new();
    private List<GameObject> _lootToSpawn = new();

    private Random _rnd;

    private List<EnemyData> _enemyList;
    private List<TresureData> _tresureList;
    private bool hasRoomBeenEntered = false;
    private List<Attachment> SpawnItems = new();


    [Header("Debug")]

    [SerializeField] private bool _debugButton;
    [SerializeField] private Generation _genOveride;
    [SerializeField] private int _maxIterations = 10000;

    [SerializeField] private bool _isEndRoom;
    [SerializeField] private GameObject[] _evilAssGlows;





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _roomData = GetComponent<RoomData>();



        if (_roomData._isEvilAssRoom)
        {
            float corLength = 5;

            foreach (var item in _roomData.ConnectedPoints)
            {
                if (item.Connected == null)
                {
                    continue;
                }
                foreach (var attachitem in _roomData.AttachedCorridors)
                {
                    if (attachitem.Item1 == item)
                    {
                        corLength = attachitem.Item1.MaxCorridorLength;
                    }
                }
            }

            float x = corLength / 10;
            foreach (var item in _evilAssGlows)
            {
                Vector3 scale = item.transform.localScale;
                scale.x = x;
                item.transform.localScale = scale;
            }
            Debug.LogWarning("Tried");
            if (_roomData.RoomAttachPoints.Length <= 0)
            {
                Debug.LogWarning("List is 0");
            }
            print("Evil");
            for (int i = 0; i < _roomData.RoomAttachPoints.Length; i++)
            {
                if (_roomData.RoomAttachPoints[i].Connected == null)
                {
                    Destroy(_evilAssGlows[i].gameObject);
                    print("Tried to Destroy" + _evilAssGlows[i].name);
                }
            }
        }
        else
        {
            if (_roomData.RoomType == RoomType.FinalRoom)
            {
                Debug.LogWarning("Tried but failed");
            }
        }


        _player = FindFirstObjectByType<PlayerMovementScript>(FindObjectsInactive.Include).gameObject;
        if (_isEndRoom)
        {
            _roomData = GetComponent<RoomData>();
            Init();
        }
    }

    public void Init()
    {
        _roomData = GetComponent<RoomData>();
        if (_genOveride != null)
        {
            _roomData.Generation = _genOveride;
        }

        _spawnPoints = _roomData.EnemySpawnPoints.ToArray();
        if (_spawnPoints == null)
        {
            throw new Exception($"Obj is missing spawnpoint at start {gameObject.name}");
        }

        _rnd = Generation.GenRnd;
        _wallTiles = Generation.Instance.GenData.WallTile;
        _wallTilemap = Generation.Instance.GlobalWallTileMap;
        //_roomData = data;
        if (_roomData.RoomType != RoomType.Treasure)
        {
            InitEnemies();
        }
        else
        {
            InitLoot();
        }




    }


    // Update is called once per frame
    void Update()
    {
        if (_debugButton)
        {
            _debugButton = false;
            Init();
            StartRoom(transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            print("Entered");
            StartRoom(collision.transform.position);
        }
    }

    void StartRoom(Vector2 postion)
    {
        if (hasRoomBeenEntered)
        {
            return;
        }
        hasRoomBeenEntered = true;
        Vector2 pos = _wallTilemap.GetCellCenterWorld(_wallTilemap.WorldToCell(postion));
        StartCoroutine(_roomData.RevealTiles(pos, AfterReveal));

        if (_roomData._isEvilAssRoom)
        {
            return;
        }
        if (_enemiesToSpawn.Count <= 0)
        {
            return;
        }

        if (_roomData.RoomType == RoomType.Treasure)
        {
            return;
        }
        if (_roomData.RoomType == RoomType.FinalRoom)
        {
            return;
        }
        CloseRoom();
    }

    void AfterReveal()
    {
        print("RevelEnded Spawning");
        print($"Roomtype: {_roomData.RoomType}");

        if (_isEndRoom)
        {
            StartBossFight();
        }

        if (_roomData.RoomType == RoomType.Treasure)
        {
            SpawnLoot();
            RoomEnd();
            return;
        }
        if (_enemiesToSpawn.Count <= 0)
        {
            RoomEnd();
            return;
        }
        if (_roomData.RoomType == RoomType.FinalRoom)
        {
            RoomEnd();
            return;
        }
        StartCoroutine(EnemyLoop());
    }

    void StartBossFight()
    {

    }


    #region EnemyMangaer

    void InitLoot()
    {
        _tresureList = Generation.Instance.GenData.TresureList.ToList();
        if (_spawnPoints == null)
        {
            throw new Exception($"Object missing spawnpoint {gameObject.name} | {_roomData.EnemySpawnPoints.Count}");
        }
        foreach (GameObject item in _spawnPoints)
        {
            TresureData tresureData = GetWeightedRandomTresure();
            _lootToSpawn.Add(tresureData.prefab);
            _tresureList.Remove(tresureData);
        }
    }

    void InitEnemies()
    {
        _enemyList = _roomData.Generation.GenData.EnemyList.ToList();
        _enemiesToSpawn = new();
        if (_roomData.Generation.GenData.EnemyList.Length <= 0)
        {
            return;
        }


        int iterator = 0;
        while (_enemyList.Count > 0 && SpawnCredits > 0)
        {
            iterator++;
            if (iterator > _maxIterations)
            {
                throw new System.Exception("Loop exceted 9000 | Init");
            }
            EnemyData data = GetWeightedRandomEnemy();
            if (data.Cost > SpawnCredits)
            {
                _enemyList.Remove(data);
            }
            else
            {
                _enemiesToSpawn.Add(data.Prefab);
                SpawnCredits -= data.Cost;
            }
        }
    }

    private int _enemiesAlive = 0;
    IEnumerator EnemyLoop()
    {

        while (_enemiesToSpawn.Count > 0 || _enemiesAlive > 0)
        {

            if (_enemiesAlive > 0) { yield return null; continue; }
            SpawnEnemies();
            yield return null;
        }

        OpenRoom();

        RoomEnd();

    }

    void SpawnLoot()
    {
        print("TRIED TO SPAWN   ");
        for (int i = 0; i < _lootToSpawn.Count; i++)
        {
            print("SPAWNED LOOT");
            Transform spawnPos = _spawnPoints[i].transform;
            Vector3 stPos = spawnPos.position;
            stPos.z = 0;
            GameObject obj = Instantiate(_lootToSpawn[i], stPos, spawnPos.rotation);
            Attachment attachment = obj.GetComponent<Attachment>();
            SpawnItems.Add(attachment);
            attachment.PickUpAction += ItemChosen;
        }
        RoomEnd();
    }
    [SerializeField] private GameObject Explosion;
    void ItemChosen(Attachment attachment)
    {
        SpawnItems.Remove(attachment);
        foreach (var item in SpawnItems)
        {
            GameObject.Instantiate(Explosion, item.transform.position, Quaternion.identity);
            Destroy(item.gameObject);

        }
    }

    void OnEnemieDeath()
    {
        _enemiesAlive--;
    }

    void SpawnEnemies()
    {
        _enemiesAlive = 0;
        List<GameObject> validSpawnPoints = _spawnPoints.ToList();
        int listLength = _enemiesToSpawn.Count;
        List<GameObject> removedPoints = new();
        foreach (var item in validSpawnPoints)
        {
            float distance = (item.transform.position - _player.transform.position).magnitude;
            if (distance < _roomData.MinDistanceToSpawn)
            {
                removedPoints.Add(item);
            }
        }
        foreach (var item in removedPoints)
        {
            validSpawnPoints.Remove(item);
        }

        for (int i = 0; i < listLength; i++)
        {
            if (i >= _roomData.MaxEnemySpawnPrWave)
            {
                return;
            }
            GameObject enemyPrefab = _enemiesToSpawn[0];
            _enemiesToSpawn.RemoveAt(0);
            GameObject obj = GetRandomValidSpawnPoint(validSpawnPoints);
            validSpawnPoints.Remove(obj);

            GameObject newEnemy = Instantiate(enemyPrefab, obj.transform.position, transform.rotation);
            //We subscribe to the death event in the enemy, so we can track when it dies65
            newEnemy.GetComponentInChildren<HealthSCript>().OnDeath += OnEnemieDeath;
            _enemiesAlive++;

            if (validSpawnPoints.Count <= 0)
            {
                return;
            }
        }
    }

    #endregion


    #region Door manager

    void CloseRoom()
    {
        SetRoomDoors(_wallTiles);
    }
    void OpenRoom()
    {
        SetRoomDoors(null);
    }

    void RoomEnd()
    {
        print("RoomEnded");
        foreach (var item in _roomData.AttachedCorridors)
        {
            if (item.Item2 == null)
            {
                continue;
            }
            if (item.Item1 == null)
            {
                continue;
            }
            //This is shit, TOO BAD
            if (!_roomData._isEvilAssRoom)
            {
                StartCoroutine(item.Item2.RevealTiles(item.Item1.transform.position, _roomData));
            }

        }
    }

    void SetRoomDoors(TileBase tile)
    {
        foreach (var attachPoint in _roomData.RoomAttachPoints)
        {
            if (attachPoint.Connected != null)
            {
                Vector3Int pos = _wallTilemap.WorldToCell(attachPoint.transform.position);
                _wallTilemap.SetTile(pos, tile);
            }
        }
    }

    #endregion

    #region Random
    EnemyData GetWeightedRandomEnemy()
    {
        float TotalWeight = 0;
        foreach (var enemyData in _enemyList)
        {
            TotalWeight += enemyData.Weight;
            //print($"{room.Object.name} | {Time.time}");
        }

        //print($"total weight {TotalWeight} | {Time.time}");

        float rndWeight = (float)_rnd.NextDouble() * TotalWeight;

        //print($"rnd weight {rndWeight} | {Time.time}");

        foreach (var enemyData in _enemyList)
        {
            rndWeight -= enemyData.Weight;
            if (rndWeight <= 0)
            {
                return enemyData;
            }
        }
        return null;
    }

    TresureData GetWeightedRandomTresure()
    {
        float TotalWeight = 0;
        foreach (var tresureData in _tresureList)
        {
            TotalWeight += tresureData.Weight;
            //print($"{room.Object.name} | {Time.time}");
        }

        //print($"total weight {TotalWeight} | {Time.time}");

        float rndWeight = (float)_rnd.NextDouble() * TotalWeight;

        //print($"rnd weight {rndWeight} | {Time.time}");

        foreach (var tresureData in _tresureList)
        {
            rndWeight -= tresureData.Weight;
            if (rndWeight <= 0)
            {
                return tresureData;
            }
        }
        return null;
    }

    GameObject GetRandomValidSpawnPoint(List<GameObject> validSpawnPoints)
    {
        int index = _rnd.Next(validSpawnPoints.Count);
        return validSpawnPoints[index];

    }
    #endregion

}

