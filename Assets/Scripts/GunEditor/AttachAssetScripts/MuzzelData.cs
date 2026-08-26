using UnityEngine;
using static AttachmentTypes;

[CreateAssetMenu(fileName = "NewMuzzel", menuName = "AttachmentData/Muzzel")]
public class MuzzelAttachment : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.Muzzel;

    public override void ApplyValues()
    {
        throw new System.NotImplementedException();
    }
}