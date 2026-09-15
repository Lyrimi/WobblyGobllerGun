using UnityEngine;
using static GunEnums;

[CreateAssetMenu(fileName = "NewMuzzel", menuName = "AttachmentData/Muzzel")]
public class MuzzelAttachment : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.Muzzel;

    public override void ApplyValues()
    {
        base.ApplyValues();
        throw new System.NotImplementedException();
    }
}