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
        print(parrent.name);
        return parrent.Children[index] == null;
    }

    public bool IsCompatiable(AttachmentType attachmentType)
    {
        return attachmentType == ValidAttachment;
    }

    // Update is called once per frame
    void Update()
    {
        if (DebugButton)
        {
            DebugButton = !DebugButton;
            print($"{IsAvailable()} | {Time.time}");
        }
    }

}
