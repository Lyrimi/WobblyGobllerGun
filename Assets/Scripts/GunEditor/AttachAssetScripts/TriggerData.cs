using UnityEngine;
using static AttachmentTypes;
[CreateAssetMenu(fileName = "NewTrigger", menuName = "AttachmentData/Trigger")]
public class TriggerAttachment : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.Trigger;
}