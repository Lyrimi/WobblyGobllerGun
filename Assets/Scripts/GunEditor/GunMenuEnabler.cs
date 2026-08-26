using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class GunMenuEnabler : MonoBehaviour
{
    [SerializeField]
    GunEditor gunEditor;
    [SerializeField]
    Animator anim;

    [SerializeField]
    Vector2 InnerSize;
    [SerializeField]
    Vector2 OuterSize;

    GameObject GEditorObj;
    InputAction EdtiorModeAction;
    bool isInEditMode = false;
    bool isInAnim = false;

    List<GameObject> ClonedObjs = new();
    List<GameObject> RealObjs = new();

    void EnableMenu()
    {
        isInAnim = true;
        StartCoroutine(EnableMenuAnim());
        isInEditMode = true;
        AddPartsToMenu();
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



            Vector2 randomPoint = new(Random.Range(-outWidth / 2, outWidth / 2), Random.Range(-outHight / 2, outHight / 2));
            if (randomPoint.x > -inWidth / 2 && randomPoint.x < inWidth / 2 && randomPoint.y > -inHight / 2 && randomPoint.y < inHight / 2)
            {
                print("Tiggered");
                if (Random.value > 0.5)
                {
                    randomPoint.x = -inWidth / 2;
                }
                else
                {
                    randomPoint.x = inWidth / 2;
                }
            }





            GameObject cloneObj = Instantiate(attachment.gameObject, gunEditor.transform);
            cloneObj.transform.localPosition = randomPoint;
            ClonedObjs.Add(cloneObj);

            Material mat = cloneObj.GetComponent<SpriteRenderer>().material;
            mat.SetInt("_Enabled", 0);

            RealObjs.Add(attachment.gameObject);
        }
    }

    void DisableMenu()
    {
        isInAnim = true;
        DropEditorObjs();
        CleanUpRealObj();
        StartCoroutine(DisableMenuAnim());
        isInEditMode = false;



    }

    List<GameObject> DelLater = new();

    void DropEditorObjs()
    {
        Attachment[] attachments = FindObjectsByType<Attachment>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (Attachment attach in attachments)
        {
            GameObject obj = attach.gameObject;
            if (ClonedObjs.Contains(obj) || obj.transform.parent != GEditorObj.transform || !attach.MoveAble)
            {
                continue;
            }

            Instantiate(obj, transform.position, Quaternion.identity, null);
            DelLater.Add(obj);


        }
    }

    void CleanUpRealObj()
    {
        for (int i = 0; i < ClonedObjs.Count; i++)
        {
            GameObject editorObj = ClonedObjs[i];
            GameObject realObj = RealObjs[i];

            Attachment attach = editorObj.GetComponent<Attachment>();

            if (attach.Parrent != null)
            {
                Destroy(realObj);
            }
        }
    }

    void CleanUpCloneObj()
    {
        for (int i = 0; i < ClonedObjs.Count; i++)
        {
            GameObject editorObj = ClonedObjs[i];
            GameObject realObj = RealObjs[i];

            Attachment attach = editorObj.GetComponent<Attachment>();

            if (attach.Parrent == null)
            {
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

        yield return new WaitForSeconds(animationLength);
        isInAnim = false;


    }

    IEnumerator DisableMenuAnim()
    {
        anim.SetTrigger("Exit");
        yield return null;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        yield return new WaitForSeconds(animationLength);
        CleanUpCloneObj();
        isInAnim = false;


    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EdtiorModeAction = InputSystem.actions.FindAction("EditMode");
        EdtiorModeAction.performed += EditButtonPressed;
        GEditorObj = gunEditor.gameObject;
    }

    void EditButtonPressed(InputAction.CallbackContext callbackContext)
    {
        if (isInAnim)
        {
            return;
        }
        if (isInEditMode)
        {
            DisableMenu();
        }
        else
        {
            EnableMenu();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Attachment attachment = collision.GetComponent<Attachment>();
        if (attachment == null || isInEditMode)
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

        Material mat = attachment.GetComponent<SpriteRenderer>().material;
        mat.SetInt("_Enabled", 0);
    }


}
