using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GunEnums;

public static class SpecialBehavourApplyer
{
    public static void ApplyBulletBehavours(GameObject bulletObj, BulletData bulletData, BulletFlying bullet)
    {
        List<BulletBehavoiur> bulletBehavoiurs = bulletData.bulletBehavoiurs;
        if (bulletBehavoiurs.Count <= 0)
        {
            return;
        }

        foreach (BulletBehavoiur behavoiur in bulletBehavoiurs)
        {
            SpecialBulletBehavoiur result = behavoiur switch
            {
                BulletBehavoiur.SplitBullet => bulletObj.AddComponent<SplitBullet>(),
                _ => null
            };


            if (result == null)
            {
                throw new System.Exception($"Tried to apply a behavoiur that doesn't exist | {behavoiur}");
            }

            bullet.onFire += result.OnFire;
            bullet.onHit += result.OnHit;
            bullet.onUpdate += result.OnUpdate;
            bullet.onDestroy += result.OnDestory;

            
        }
    }
}
