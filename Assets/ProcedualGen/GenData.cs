using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static RoomTypes;

[CreateAssetMenu(fileName = "GenData", menuName = "Generation/GenData")]
public class GenData : ScriptableObject
{
    [SerializeField] public WeightedObject[] Objects;

    [SerializeField] private Qouta[] _quotas;

    [SerializeField] public RoomType[] CleanUpTypes;

    [SerializeField] public GameObject StartRoom;
    [SerializeField] public GameObject EndRoom;

    [SerializeField] public TileBase WallTile;

    [SerializeField] public EnemyData[] EnemyList;
    [SerializeField] public TresureData[] TresureList;

    [SerializeField] public float DifScalingMultiplyer;
    [SerializeField] public int StartingCredits;

    void Awake()
    {

    }

    public Dictionary<RoomType, int> BuildDict()
    {
        Dictionary<RoomType, int> qouta = new();

        foreach (var item in _quotas)
        {
            qouta.Add(item.Type, item.Count);
        }
        return qouta;
    }

}


[Serializable]
public class Qouta
{
    public RoomType Type;
    public int Count;

    public Qouta(RoomType _Type, int _Count)
    {
        Type = _Type;
        Count = _Count;
    }
}

[Serializable]
public class WeightedObject
{
    public GameObject Object;
    public int Weight = 1;
}
[Serializable]
public class EnemyData
{
    public GameObject Prefab;
    public int Cost;
    public float Weight;
}
[Serializable]
public class TresureData
{
    public GameObject prefab;
    public float Weight;
}