using UnityEngine;
using static GunEnums;

[CreateAssetMenu(fileName = "NewMuzzel", menuName = "AttachmentData/Muzzel")]
public class MuzzelAttachment : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.Muzzel;
    public int ProjectileMult = 1;
    public int ProjectileAdd = 0;

    public override void ApplyValues()
    {
        base.ApplyValues();
        GunData.bulletData.Projectiles += ProjectileAdd;
        GunData.bulletData.Projectiles *= ProjectileMult;

    }
}