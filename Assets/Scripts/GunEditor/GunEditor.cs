using System;
using System.Linq;
using Pathfinding.Util;
using Unity.Loading;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunEditor : MonoBehaviour
{
    [SerializeField]
    Transform GunEditorParrent;
    InputAction pointAction;
    InputAction clickAction;

    GameObject curObj;
    Attachment curAttach;

    AttachmentPoint curSnap;

    [SerializeField]
    float MaxSnapingDistance;
    [SerializeField]
    Attachment MainAttachmentParrent;

    AttachmentPoint[] attachmentPoints;

    LineRenderer lrend;

    [SerializeField] StatTextManager _statTextManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void CheckIfClicked(InputAction.CallbackContext callbackContext)
    {

        //print($"{} | {Time.time}");


        Vector3 screenPos = Pointer.current.position.ReadValue();
        screenPos.z = -Camera.main.transform.position.z;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        Debug.DrawRay(worldPos, Vector2.zero, Color.blue);

        if (hit.collider != null)
        {

            Attachment attachment = hit.collider.gameObject.GetComponent<Attachment>();

            if (attachment == null || !attachment.MoveAble)
            {
                return;
            }
            Click(attachment);
        }
    }

    void Click(Attachment attachment)
    {
        if (attachment.transform.root != GunEditorParrent.root)
        {
            return;
        }
        if (attachment.Parrent != null)
        {
            attachment.Parrent.RemoveChild(attachment.index, GunEditorParrent);
            RecalcData();
        }
        curObj = attachment.gameObject;
        curAttach = attachment;
        StartHighlight();
    }

    void Drop()
    {
        lrend.enabled = false;
        StopHighlight();
        if (curSnap != null)
        {
            SnapAttach();
        }
        curObj = null;
        curAttach = null;
        _statTextManager.ClearText();
    }

    void SnapAttach()
    {
        curSnap.parrent.TryAddChild(curAttach, curSnap.index);
        RecalcData();

    }

    void Drag()
    {
        Vector3 screenPos = Pointer.current.position.ReadValue();


        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = curObj.transform.parent.position.z;
        curObj.transform.position = worldPos;
        lrend.enabled = false;
        if (curAttach.attachmentData.AttachmentType == GunEnums.AttachmentType.Trigger && MainAttachmentParrent.attachmentPoints[0].IsAvailable())
        {
            lrend.enabled = true;
            lrend.SetPosition(0, curAttach.gameObject.transform.position);
            lrend.SetPosition(1, MainAttachmentParrent.gameObject.transform.position);
        }



        if (curAttach.ConnectingPoint == null)
        {
            return;
        }

        CheckNearAttachPoint(curAttach.ConnectingPoint.position);
    }

    void CheckNearAttachPoint(Vector2 snapPos)
    {
        float distance = MaxSnapingDistance + 1;
        int index = -1;
        for (int i = 0; i < attachmentPoints.Length; i++)
        {
            var point = attachmentPoints[i];
            if (point.parrent == curAttach)
            {
                continue;
            }
            Vector2 attachpos = point.transform.position;
            float lDistance = Vector2.Distance(attachpos, snapPos);
            if (lDistance < distance)
            {
                distance = lDistance;
                index = i;
            }
        }

        if (distance < MaxSnapingDistance)
        {
            if (attachmentPoints[index].IsCompatiable(curAttach.attachmentData.AttachmentType))
            {
                HighlightSnap(index);
                curSnap = attachmentPoints[index];
                return;
            }

        }
        curSnap = null;
        HightLightReset();

    }


    void HightLightReset()
    {
        foreach (AttachmentPoint point in attachmentPoints)
        {
            if (point.parrent == curAttach)
            {
                continue;
            }
            SpriteRenderer rend = point.GetComponent<SpriteRenderer>();
            rend.color = Color.yellow;

        }
    }

    void HighlightSnap(int index)
    {

        var point = attachmentPoints[index];
        SpriteRenderer rend = point.GetComponent<SpriteRenderer>();
        rend.color = Color.red;


    }

    void StartHighlight()
    {
        attachmentPoints = FindObjectsByType<AttachmentPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (AttachmentPoint point in attachmentPoints)
        {
            if (point.parrent == null)
            {
                Debug.LogWarning($"Point: {point} missing parrent asigment ... skipping | More info, point parrent: {point.transform.parent.name} | point name: {point.name}");
                continue;
            }
            if (point.transform.root != GunEditorParrent.root)
            {
                continue;
            }
            //print(point.transform.parent.name);
            if (point.IsAvailable() && point.IsCompatiable(curAttach.attachmentData.AttachmentType))
            {
                if (point.parrent == curAttach)
                {
                    continue;
                }
                SpriteRenderer rend = point.GetComponent<SpriteRenderer>();
                rend.color = Color.yellow;
                rend.enabled = true;
            }
        }
    }

    void StopHighlight()
    {
        foreach (AttachmentPoint point in attachmentPoints)
        {
            SpriteRenderer rend = point.GetComponent<SpriteRenderer>();
            rend.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (curObj != null && !clickAction.IsPressed())
        {
            Drop();
        }

        if (curObj != null)
        {
            Drag();
        }

        CheckIfHovering();

    }

    void CheckIfHovering()
    {
        Vector3 screenPos = Pointer.current.position.ReadValue();
        screenPos.z = -Camera.main.transform.position.z;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Physics2D.SyncTransforms();
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        Debug.DrawRay(worldPos, Vector2.zero, Color.blue);

        if (hit.collider != null)
        {

            Attachment attachment = hit.collider.gameObject.GetComponent<Attachment>();

            if (attachment == null || !attachment.MoveAble)
            {
                if (curMat != null)
                {
                    curMat.SetInt("_Enabled", 0);
                    _statTextManager.ClearText();
                }
                return;
            }

            HoverHighLight(attachment.gameObject);

            if (attachment.attachmentData.StatDisplays.Length > 0)
            {
                if (curObj == null)
                {
                    _statTextManager.SetText(attachment.attachmentData.StatDisplays);
                    _statTextManager.SetPostion(attachment.transform.position);
                }
                else
                {
                    _statTextManager.ClearText();
                }

            }

        }
        else
        {
            if (curMat != null)
            {
                curMat.SetInt("_Enabled", 0);
                _statTextManager.ClearText();
            }
        }
    }
    // _Enabled
    Material curMat;
    void HoverHighLight(GameObject obj)
    {
        if (curMat != null)
        {
            curMat.SetInt("_Enabled", 0);
        }

        curMat = obj.GetComponent<SpriteRenderer>().material;
        curMat.SetInt("_Enabled", 1);
    }

    void Awake()
    {
        pointAction = InputSystem.actions.FindAction("Point");
        clickAction = InputSystem.actions.FindAction("Click");
        clickAction.performed += CheckIfClicked;
        lrend = MainAttachmentParrent.GetComponent<LineRenderer>();
        //_statTextManager = GetComponentInChildren<StatTextManager>();
    }

    public void RecalcData()
    {

        GunData.bulletData = new();
        GunData.FireRate = 0;
        GunData.TurnSpeedMult = 1;
        GunData.TopDownGunPrefab = null;
        GunData.TopDownGunObj = null;
        GunData.bulletData.Damage = 0;
        GunData.bulletData.LifeTime = 0;
        GunData.bulletData.Spreed = 0;
        GunData.bulletData.Projectiles = 0;
        GunData.GunBehavoiurs = new();
        GunData.audioClip = null;
        GunData.AudioVolume = 1;

        if (MainAttachmentParrent.Children.Length <= 0)
        {
            return;
        }
        Attachment mainAttachment = MainAttachmentParrent.Children[0];
        if (mainAttachment == null)
        {
            return;
        }
        if (mainAttachment.attachmentData is TriggerAttachment triggerAttachment)
        {
            //print("ran");
            mainAttachment.CalcDataWithChildren();
        }


        GunStats[] gunStats = FindObjectsByType<GunStats>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var stats in gunStats)
        {
            stats.UpdateText();
        }
    }
}
