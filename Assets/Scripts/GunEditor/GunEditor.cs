using System;
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

    AttachmentPoint[] attachmentPoints;
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
        if (attachment.Parrent != null)
        {
            attachment.Parrent.RemoveChild(attachment.index, GunEditorParrent);
        }
        curObj = attachment.gameObject;
        curAttach = attachment;
        StartHighlight();
    }

    void Drop()
    {
        StopHighlight();
        if (curSnap != null)
        {
            SnapAttach();
        }
        curObj = null;
        curAttach = null;
    }

    void SnapAttach()
    {
        curSnap.parrent.TryAddChild(curAttach, curSnap.index);

    }

    void Drag()
    {
        Vector3 screenPos = Pointer.current.position.ReadValue();


        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = curObj.transform.parent.position.z;
        curObj.transform.position = worldPos;


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
            HighlightSnap(index);
            curSnap = attachmentPoints[index];
        }
        else
        {
            curSnap = null;
            HightLightReset();
        }

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
            print(point.transform.parent.name);
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
                }
                return;
            }

            HoverHighLight(attachment.gameObject);

        }
        else
        {
            if (curMat != null)
            {
                curMat.SetInt("_Enabled", 0);
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

    void Start()
    {
        pointAction = InputSystem.actions.FindAction("Point");
        clickAction = InputSystem.actions.FindAction("Click");
        clickAction.performed += CheckIfClicked;
    }
}
