using System;
using UnityEngine;
using static GunEnums;

public class Attachment : MonoBehaviour
{

    public AttachmentData attachmentData;

    public AttachmentPoint[] attachmentPoints;

    public bool MoveAble = true;

    public Action<Attachment> PickUpAction;


    public Transform ConnectingPoint;

    [SerializeField] private bool _debugButton;

    [Header("DO NOT TOUCH")]
    public Attachment[] Children;
    public Attachment Parrent;

    [NonSerialized]
    public int index = 0;

    public bool TryAddChild(Attachment attachment, int index)
    {

        if (Children[index] != null)
        {
            return false;
        }
        Children[index] = attachment;
        attachment.Parrent = this;
        attachment.index = index;

        Redraw();
        return true;

    }

    public void RemoveChild(int index, Transform newParrent)
    {
        if (Children[index] == null)
        {
            return;
        }
        Attachment child = Children[index];
        child.Parrent = null;
        Children[index] = null;
        child.transform.parent = newParrent;
        child.index = 0;
        child.Redraw();

    }

    public void Redraw()
    {
        for (int i = 0; i < Children.Length; i++)
        {
            Attachment child = Children[i];
            if (child == null)
            {
                continue;
            }
            Vector3 ConnectedOfset = child.ConnectingPoint.localPosition;
            Vector3 AttachPointPos = attachmentPoints[i].transform.position;

            Vector3 finalPos = AttachPointPos - ConnectedOfset;

            child.transform.position = finalPos;

            child.transform.parent = transform;

            child.Redraw();



        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Children = new Attachment[attachmentPoints.Length];
        if (attachmentPoints.Length <= 0)
        {
            return;
        }
        for (int i = 0; i < attachmentPoints.Length; i++)
        {
            AttachmentPoint point = attachmentPoints[i];
            point.parrent = this;
            point.index = i;
        }
    }

    public void CalcDataWithChildren()
    {
        attachmentData.ApplyValues();
        foreach (var child in Children)
        {
            if (child != null)
            {
                child.CalcDataWithChildren();
            }
        }
    }

    public void TestChildDataType()
    {
        SpecialBulletBehavoiur specialBulletBehavoiur;
        specialBulletBehavoiur = GetComponent<SpecialBulletBehavoiur>();

        print($"{specialBulletBehavoiur}");
        Type polyType = specialBulletBehavoiur.GetType();

        print($"{polyType.Name}");
    }

    // Update is called once per frame
    void Update()
    {
        if (_debugButton)
        {
            _debugButton = false;
            TestChildDataType();
        }
    }
}
