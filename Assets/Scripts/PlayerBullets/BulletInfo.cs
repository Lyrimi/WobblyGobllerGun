using UnityEngine;

[CreateAssetMenu(fileName = "BuletData", menuName = "Game/BuletData")]
public class BulletInfo : ScriptableObject
{
    public GameObject BulletPrefab;

    public static BulletInfo Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this;
    }
}