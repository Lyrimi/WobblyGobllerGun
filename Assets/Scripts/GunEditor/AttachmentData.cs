using System;
using Unity.VisualScripting;
using UnityEngine;
using static AttachmentTypes;


public abstract class AttachmentData : ScriptableObject
{
    public abstract AttachmentType AttachmentType { get; }
}
[CreateAssetMenu(fileName = "NewBarrel", menuName = "AttachmentData/Barrel")]
public class BarrelAttachment : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.Barrel;
    public float DamageMod;
}

[CreateAssetMenu(fileName = "NewMuzzel", menuName = "AttachmentData/Muzzel")]
public class MuzzelAttachment : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.Muzzel;
}

[CreateAssetMenu(fileName = "NewTrigger", menuName = "AttachmentData/Trigger")]
public class TriggerAttachment : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.Trigger;
}

[CreateAssetMenu(fileName = "NewStock", menuName = "AttachmentData/Stock")]
public class StockAttachment : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.Stock;
}
