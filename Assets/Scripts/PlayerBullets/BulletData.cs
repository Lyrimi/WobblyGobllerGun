using System;
using UnityEngine;
using static AttachmentTypes;

public class BulletData
{
    public float Damage;
    public float Spreed;
    public float Speed;
    public int Projectiles;
    public float LifeTime;



    public void SpawnBullet()
    {
        if (GunData.TopDownGunObj == null)
        {
            Debug.Log("Can't fire: No Barrel");
            return;
        }
        Transform[] SpawnPoints = GunData.TopDownGunObj.GetComponent<TopDownGunData>().EndPoints;

        foreach (var Point in SpawnPoints)
        {
            if (BulletInfo.Instance == null)
            {
                Debug.LogError("Missing BulletInfo Obj");
                return;
            }
            GameObject obj = GameObject.Instantiate(BulletInfo.Instance.BulletPrefab);
            obj.transform.position = Point.transform.position;
            obj.transform.rotation = Point.transform.rotation;
            obj.GetComponent<BulletFlying>().Fire();
        }
    }

}

public static class GunData
{
    public static float FireRate;
    public static float TurnSpeedMult;
    public static GameObject TopDownGunPrefab;
    public static GameObject TopDownGunObj;

    public static BulletData bulletData = new();
}