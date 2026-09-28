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

[RequireComponent(typeof(RoomData))]
public class RoomDirector : MonoBehaviour
{
    private RoomData _roomData;
    private TileBase _wallTiles;
    private Tilemap _wallTilemap;

    private GameObject[] _spawnPoints;



    [SerializeField] public int SpawnCredits = 0;

    private List<GameObject> _enemiesToSpawn;
    private List<GameObject> _lootToSpawn;

    private Random _rnd;

    private List<EnemyData> _enemyList;
    private List<TresureData> _tresureList;


    [Header("Debug")]

    [SerializeField] private bool _debugButton;
    [SerializeField] private Generation _genOveride;
    [SerializeField] private int _maxIterations = 10000;





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _roomData = GetComponent<RoomData>();
        if (_genOveride != null)
        {
            _roomData.Generation = _genOveride;
        }

        _spawnPoints = _roomData.EnemySpawnPoints.ToArray();


    }

    public void Init(RoomData data)
    {
        _rnd = Generation.GenRnd;
        _wallTiles = Generation.Instance.GenData.WallTile;
        _wallTilemap = Generation.Instance.GlobalWallTileMap;
        _roomData = data;

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
            Init(_roomData);
            StartRoom(transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        print("Entered");
        if (collision.CompareTag("Player"))
        {
            StartRoom(collision.transform.position);
        }
    }

    void StartRoom(Vector2 postion)
    {
        print(postion);
        Vector2 pos = _wallTilemap.CellToWorld(_wallTilemap.WorldToCell(postion));
        StartCoroutine(_roomData.RevealTiles(pos));

        if (SpawnCredits <= 0)
        {
            return;
        }

        if (_roomData.RoomType != RoomType.Treasure)
        {
            StartCoroutine(EnemyLoop());

        }
        else
        {
            SpawnLoot();
        }

    }


    #region EnemyMangaer

    void InitLoot()
    {
        foreach (GameObject item in _spawnPoints)
        {
            _lootToSpawn.Add(GetWeightedRandomTresure().prefab);
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
        CloseRoom();
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
        for (int i = 0; i < _lootToSpawn.Count; i++)
        {
            Transform spawnPos = _spawnPoints[i].transform;
            Instantiate(_lootToSpawn[i], spawnPos.position, spawnPos.rotation);
        }
        RoomEnd();
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
        int iterator = 0;
        for (int i = 0; i < listLength; i++)
        {
            iterator++;
            if (iterator > _maxIterations)
            {
                throw new System.Exception("Loop exceted 9000 | SEnemies");
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
        foreach (var item in _roomData.AttachedCorridors)
        {
            //This is shit, TOO BAD
            StartCoroutine(item.Item2.RevealTiles(item.Item1.transform.position));
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

