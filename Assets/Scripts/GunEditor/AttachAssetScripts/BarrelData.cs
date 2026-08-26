using UnityEngine;
using static AttachmentTypes;
[CreateAssetMenu(fileName = "NewBarrel", menuName = "AttachmentData/Barrel")]
public class BarrelAttachment : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.Barrel;
    public float DamageMod;
}