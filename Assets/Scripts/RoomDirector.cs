using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class RoomDirector : MonoBehaviour
{
    RoomData _roomData;
    TileBase _wallTiles;
    Tilemap _wallTilemap;

    GameObject[] _spawnPoints;



    int _spawnCredits = 0;

    List<GameObject> _enemiesToSpawn;

    Random _rnd;

    List<EnemyData> _enemyList;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        _rnd = Generation.GenRnd;
        _roomData = GetComponent<RoomData>();
        _spawnPoints = _roomData.EnemySpawnPoints.ToArray();
        _wallTiles = _roomData.Generation.GenData.WallTile;
        _wallTilemap = _roomData.Generation.GlobalWallTileMap;

        _enemyList = _roomData.Generation.GenData.EnemyList.ToList();
    }

    public void Init()
    {
        InitEnemies();
    }


    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartRoom();
        }
    }

    void StartRoom()
    {
        CloseRoom();
    }


    #region EnemyMangaer

    void InitEnemies()
    {
        while (_enemyList.Count > 0 && _spawnCredits > 0)
        {
            EnemyData data = GetWeightedRandomEnemy();
            if (data.Cost > _spawnCredits)
            {
                _enemyList.Remove(data);
            }
            else
            {
                _enemiesToSpawn.Add(data.Prefab);
                _spawnCredits -= data.Cost;
            }
        }
    }

    private int _enemiesAlive = 0;
    IEnumerable EnemyLoop()
    {
        while (_enemiesToSpawn.Count > 0)
        {
            if (_enemiesAlive > 0) { continue; }
            SpawnEnemies();
            yield return null;
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

        for (int i = 0; i < listLength; i++)
        {
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

    GameObject GetRandomValidSpawnPoint(List<GameObject> validSpawnPoints)
    {
        int index = _rnd.Next(validSpawnPoints.Count);
        return validSpawnPoints[index];

    }
    #endregion

}

