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
    }

    public BulletBehavoiur[] BulletBehavoiurs;
}






