using UnityEngine;
using static AttachmentTypes;



[CreateAssetMenu(fileName = "NewStock", menuName = "AttachmentData/Stock")]
public class StockAttachment : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.Stock;
    public float SpreedMult;
    public float TurnSpeedMult;

    public override void ApplyValues()
    {
        GunData.bulletData.Spreed = GunData.bulletData.Spreed * SpreedMult;
        GunData.TurnSpeedMult += TurnSpeedMult;
    }
}
