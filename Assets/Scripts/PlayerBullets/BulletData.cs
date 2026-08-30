using System;
using UnityEngine;
using static AttachmentTypes;
using Random = UnityEngine.Random;

public class BulletData
{
    public float Damage;
    public float Spreed;
    public float Speed;
    public int Projectiles;
    public float LifeTime;



    public void ShootBullets()
    {
        if (GunData.TopDownGunObj == null)
        {
            Debug.Log("Can't fire: No Barrel");
            return;
        }
        if (BulletInfo.Instance == null)
        {
            Debug.LogError("Missing BulletInfo Obj");
            return;
        }
        for (int i = 0; i < Projectiles; i++)
        {
            Fire();
        }
    }

    void Fire()
    {
        Transform[] SpawnPoints = GunData.TopDownGunObj.GetComponent<TopDownGunData>().EndPoints;

        foreach (var Point in SpawnPoints)
        {

            GameObject obj = GameObject.Instantiate(BulletInfo.Instance.BulletPrefab);
            float RandomSpreed = Random.Range(-Spreed / 2, Spreed / 2);
            obj.transform.position = Point.transform.position;
            obj.transform.rotation = Point.transform.rotation;
            obj.transform.Rotate(new(0, 0, RandomSpreed));
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