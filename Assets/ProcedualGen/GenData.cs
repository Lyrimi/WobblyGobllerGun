using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static RoomTypes;

[CreateAssetMenu(fileName = "GenData", menuName = "Generation/GenData")]
public class GenData : ScriptableObject
{
    public GameObject[] Objects;
    public List<Qouta> Quotas;


}

[Serializable]
public class Qouta
{
    public RoomType Type;
    public int Count;
}
