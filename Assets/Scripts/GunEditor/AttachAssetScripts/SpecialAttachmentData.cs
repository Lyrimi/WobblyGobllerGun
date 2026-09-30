using UnityEngine;
using static GunEnums;

[CreateAssetMenu(fileName = "NewSpecialAttachment", menuName = "AttachmentData/SpecialAttachment")]
public class SpecialAttachmentData : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.SpecialAttachment;
    public override void ApplyValues()
    {
        base.ApplyValues();

    }
}
