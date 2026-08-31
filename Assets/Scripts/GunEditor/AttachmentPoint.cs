using System;
using UnityEngine;

using static AttachmentTypes;
public class AttachmentPoint : MonoBehaviour
{
    public AttachmentType ValidAttachment;

    [NonSerialized]
    public int index;
    [NonSerialized]
    public Attachment parrent;

    public bool DebugButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public bool IsAvailable()
    {
        if (parrent == null)
        {
            Debug.LogWarning($"Point: {gameObject} missing parrent asigment ... skipping");
            return false;
        }
        return parrent.Children[index] == null;
    }

    public bool IsCompatiable(AttachmentType attachmentType)
    {
        if (parrent == null)
        {
            Debug.LogWarning($"Point: {gameObject} missing parrent asigment ... skipping");
            return false;
        }
        return attachmentType == ValidAttachment;
    }

    // Update is called once per frame
    void Update()
    {
        if (DebugButton)
        {
            DebugButton = !DebugButton;
            //print($"{IsAvailable()} | {Time.time}");
        }
    }

}
