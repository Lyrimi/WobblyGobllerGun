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
        Transform[] SpawnPoints = GunData.TopDownGun.GetComponent<TopDownGunData>().EndPoints;
    }

}

public static class GunData
{
    public static float FireRate;
    public static float TurnSpeedMult;
    public static GameObject TopDownGun;

    public static BulletData bulletData;
}