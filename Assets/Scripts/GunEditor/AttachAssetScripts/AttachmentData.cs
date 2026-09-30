using System;
using System.ComponentModel;
using UnityEngine;
using static GunEnums;


public abstract class AttachmentData : ScriptableObject
{
    public abstract AttachmentType AttachmentType { get; }

    public virtual void ApplyValues()
    {
        foreach (var bulletBehavoiur in BulletBehavoiurs)
        {
            GunData.bulletData.bulletBehavoiurs.Add(bulletBehavoiur);
        }
        GunData.TurnSpeedMult *= TurnSpeedMult;
    }

    public BulletBehavoiur[] BulletBehavoiurs;
    public StatDisplay[] StatDisplays;

    public float TurnSpeedMult = 1;
}

[Serializable]
public class StatDisplay
{
    public Color TextColor;
    public string Text;
}



