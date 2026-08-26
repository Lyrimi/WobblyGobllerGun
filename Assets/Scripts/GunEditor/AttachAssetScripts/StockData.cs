using UnityEngine;
using static AttachmentTypes;



[CreateAssetMenu(fileName = "NewStock", menuName = "AttachmentData/Stock")]
public class StockAttachment : AttachmentData
{
    public override AttachmentType AttachmentType => AttachmentType.Stock;
}
