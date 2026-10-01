using System;
using Unity.VisualScripting;
using UnityEngine;

public class TopDownGunData : MonoBehaviour
{
    public Transform[] EndPoints;
    public Transform Connector;

    public Action OnInit;
    public Action OnTick;
    public Action OnDestroyObj;

    void Start()
    {
        OnInit?.Invoke();
    }

    void Update()
    {
        OnTick?.Invoke();
    }

    void OnDestroy()
    {
        OnDestroyObj?.Invoke();
    }

}
