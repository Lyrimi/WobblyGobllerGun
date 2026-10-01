using JetBrains.Annotations;
using UnityEngine;
using static GunEnums;
[CreateAssetMenu(fileName = "NewTrigger", menuName = "AttachmentData/Trigger")]
public class TriggerAttachment : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.Trigger;

    public float BaseDmg;
    public float FireRate;
    public float BaseSpreed;
    public int Projectiles;
    public AudioClip AudioClip;
    [Range(0f, 1f)]
    public float AudioVolume = 1;

    public override void ApplyValues()
    {
        base.ApplyValues();
        GunData.bulletData.Damage = BaseDmg;
        GunData.bulletData.Spreed = BaseSpreed;
        GunData.FireRate = FireRate;
        GunData.bulletData.Projectiles = Projectiles;
        GunData.audioClip = AudioClip;
        GunData.AudioVolume = AudioVolume;
    }
}