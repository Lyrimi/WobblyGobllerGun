using UnityEngine;
using static AttachmentTypes;


public abstract class AttachmentData : ScriptableObject
{
    public abstract AttachmentType AttachmentType { get; }
    public abstract void ApplyValues();
}






