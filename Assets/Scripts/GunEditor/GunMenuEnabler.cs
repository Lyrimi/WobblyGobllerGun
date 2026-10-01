using System;
using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class GunMenuEnabler : MonoBehaviour
{
    [SerializeField]
    GameObject GunObj;
    [SerializeField]
    Transform GunAttachPoint;
    [SerializeField]
    GunEditor gunEditor;
    [SerializeField]
    Animator anim;

    [SerializeField]
    Vector2 InnerSize;
    [SerializeField]
    Vector2 OuterSize;
    [SerializeField]
    Camera MainCam;

    GameObject GEditorObj;
    InputAction EdtiorModeAction;
    bool isInEditMode = false;
    bool isInAnim = false;

    List<GameObject> ClonedObjs = new();
    List<GameObject> RealObjs = new();

    void EnableMenu()
    {
        gunEditor.RecalcData();
        isInAnim = true;
        StartCoroutine(EnableMenuAnim());
        isInEditMode = true;
        AddPartsToMenu();
        Time.timeScale = 0;
    }

    void DisableMenu()
    {
        isInAnim = true;
        gunEditor.RecalcData();
        RebuildGun();
        DropEditorObjs();
        CleanUpClonebObj();
        StartCoroutine(DisableMenuAnim());
        isInEditMode = false;
        Time.timeScale = 1;
    }

    void AddPartsToMenu()
    {
        Collider2D[] coliders = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius);

        float outWidth = OuterSize[0];
        float outHight = OuterSize[1];
        float inWidth = InnerSize[0];
        float inHight = InnerSize[1];

        foreach (var col in coliders)
        {
            Attachment attachment = col.GetComponent<Attachment>();
            if (attachment == null)
            {
                continue;
            }
            if (attachment.Parrent != null)
            {
                continue;
            }



            Vector2 randomPoint = new(Random.Range(-outWidth / 2, outWidth / 2), Random.Range(-outHight / 2, outHight / 2));
            if (randomPoint.x > -inWidth / 2 && randomPoint.x < inWidth / 2 && randomPoint.y > -inHight / 2 && randomPoint.y < inHight / 2)
            {
                //print("Tiggered");
                if (Random.value > 0.5)
                {
                    randomPoint.x = -inWidth / 2;
                }
                else
                {
                    randomPoint.x = inWidth / 2;
                }
            }



            //GameObject cloneObj = Instantiate(attachment.gameObject, gunEditor.transform);

            GameObject cloneObj = Instantiate(attachment.gameObject, attachment.transform.position, attachment.transform.rotation);

            ClonedObjs.Add(cloneObj);
            Attachment attachobj = cloneObj.GetComponent<Attachment>();
            Destroy(attachobj);

            attachment.transform.parent = gunEditor.transform;
            attachment.transform.localPosition = randomPoint;
            Material mat = attachment.GetComponent<SpriteRenderer>().material;
            mat.SetInt("_Enabled", 0);

            RealObjs.Add(attachment.gameObject);
        }
    }



    List<GameObject> DelLater = new();

    void DropEditorObjs()
    {
        Attachment[] attachments = FindObjectsByType<Attachment>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (Attachment attach in attachments)
        {
            GameObject obj = attach.gameObject;
            if (RealObjs.Contains(obj) || obj.transform.parent != GEditorObj.transform || !attach.MoveAble)
            {
                continue;
            }

            GameObject tempObj = Instantiate(obj, obj.transform.position, obj.transform.rotation, obj.transform.parent);
            Attachment attachobj = tempObj.GetComponent<Attachment>();
            Destroy(attachobj);
            DelLater.Add(tempObj);
            obj.transform.position = transform.position;
            obj.transform.parent = null;


        }
    }

    void CleanUpClonebObj()
    {
        for (int i = 0; i < ClonedObjs.Count; i++)
        {
            GameObject CloneObj = ClonedObjs[i];
            GameObject realObj = RealObjs[i];

            Attachment attach = realObj.GetComponent<Attachment>();

            if (attach.Parrent != null)
            {
                Destroy(CloneObj);
            }
        }
    }

    void CleanUpRealObj()
    {
        for (int i = 0; i < ClonedObjs.Count; i++)
        {
            GameObject editorObj = ClonedObjs[i];
            GameObject realObj = RealObjs[i];

            Attachment attach = realObj.GetComponent<Attachment>();

            if (attach.Parrent == null)
            {
                realObj.transform.position = editorObj.transform.position;
                realObj.transform.rotation = editorObj.transform.rotation;
                realObj.transform.parent = editorObj.transform.parent;
                Destroy(editorObj);
            }
        }

        foreach (var obj in DelLater)
        {
            Destroy(obj);
        }

        DelLater = new();
        ClonedObjs = new();
        RealObjs = new();
    }



    IEnumerator EnableMenuAnim()
    {



        anim.SetTrigger("Enter");
        yield return null;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        yield return new WaitForSecondsRealtime(animationLength);
        isInAnim = false;


    }

    IEnumerator DisableMenuAnim()
    {
        anim.SetTrigger("Exit");
        yield return null;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        yield return new WaitForSecondsRealtime(animationLength);
        CleanUpRealObj();
        isInAnim = false;


    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EdtiorModeAction = InputSystem.actions.FindAction("EditMode");
        EdtiorModeAction.performed += EditButtonPressed;
        GEditorObj = gunEditor.gameObject;
        RebuildGun();
    }

    void EditButtonPressed(InputAction.CallbackContext callbackContext)
    {
        print("EditButtonPressed");
        print(isInEditMode);
        print(isInAnim);
        if (isInAnim)
        {
            return;
        }
        if (isInEditMode)
        {
            print("Disable Ran");
            DisableMenu();
        }
        else
        {
            print("Enable Ran");
            EnableMenu();
        }
    }
    //bool is to check if the gun failed to build
    public Action<bool> OnGunRebuild;

    void RebuildGun()
    {
        if (GunData.TopDownGunPrefab == null)
        {
            Debug.Log("Barrel Missing, Can't fire");
            Destroy(GunObj);
            GunObj = null;
            OnGunRebuild?.Invoke(false);
            return;
        }

        Vector3 ConnectedOfset = GunData.TopDownGunPrefab.GetComponent<TopDownGunData>().Connector.localPosition;
        Vector3 AttachPointPos = GunAttachPoint.transform.localPosition;

        Vector3 finalPos = AttachPointPos - ConnectedOfset;


        GameObject newObj = Instantiate(
        GunData.TopDownGunPrefab,
        position: finalPos,
        rotation: transform.rotation,
        parent: transform
        );
        newObj.transform.localPosition = finalPos;
        Destroy(GunObj);
        GunObj = newObj;
        GunData.TopDownGunObj = newObj;
        OnGunRebuild?.Invoke(true);
        SpecialBehavourApplyer.ApplyGunBehavoiurs(newObj.GetComponent<TopDownGunData>());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        print("Entered Something");
        Attachment attachment = collision.GetComponent<Attachment>();
        if (attachment == null || isInEditMode)
        {
            return;
        }
        if (attachment.Parrent != null)
        {
            return;
        }

        Material mat = attachment.GetComponent<SpriteRenderer>().material;
        mat.SetInt("_Enabled", 1);
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        Attachment attachment = collision.GetComponent<Attachment>();
        if (attachment == null || isInEditMode)
        {
            return;
        }
        if (attachment.Parrent != null)
        {
            return;
        }

        Material mat = attachment.GetComponent<SpriteRenderer>().material;
        mat.SetInt("_Enabled", 0);
    }


}
