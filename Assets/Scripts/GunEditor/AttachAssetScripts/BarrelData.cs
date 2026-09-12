using UnityEngine;
using static GunEnums;
[CreateAssetMenu(fileName = "NewBarrel", menuName = "AttachmentData/Barrel")]
public class BarrelAttachment : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.Barrel;
    public float DamageMult;
    public float SpreedMult;
    public float LifeTime;
    public float Speed;
    public GameObject TopDownBarrelPrefab;

    public override void ApplyValues()
    {
        BulletData BData = GunData.bulletData;
        BData.Damage *= DamageMult;
        BData.Spreed *= SpreedMult;
        BData.LifeTime += LifeTime;
        BData.Speed += Speed;
        GunData.TopDownGunPrefab = TopDownBarrelPrefab;

    }
}