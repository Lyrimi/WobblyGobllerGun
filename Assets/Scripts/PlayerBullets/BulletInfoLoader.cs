using UnityEngine;

public class BulletInfoLoader : MonoBehaviour
{
    [SerializeField]
    BulletInfo _bulletInfoInstance;
    void Awake()
    {
        BulletInfo.Instance = _bulletInfoInstance;
    }
}
