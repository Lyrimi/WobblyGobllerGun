using UnityEngine;
using UnityEngine.InputSystem;

public class CheatCheat : MonoBehaviour
{
    int curIndex;

    GameObject[] ObjectsToSpawn;

    bool isPart2;

    Vector2[] answers = new Vector2[]
    {
        Vector2.up,
        Vector2.up,
        Vector2.down,
        Vector2.down,
        Vector2.left,
        Vector2.right,
        Vector2.left,
        Vector2.right,

        Vector2.down,
        Vector2.up,
        Vector2.left,
    };
    InputAction KonamiA = InputSystem.actions.FindAction("KonamiCodeA");
    InputAction KonamiB = InputSystem.actions.FindAction("KonamiCodeB");
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        KonamiA.performed += partA;
        KonamiB.performed += partB;
    }

    void partA(InputAction.CallbackContext callbackContext)
    {
        if (curIndex > 7)
        {
            curIndex = 0;
            return;
        }
        if (callbackContext.ReadValue<Vector2>() == answers[curIndex])
        {
            curIndex++;
        }
        else
        {
            curIndex = 0;
        }
    }

    void partB(InputAction.CallbackContext callbackContext)
    {
        if (curIndex < 8)
        {
            curIndex = 0;
            return;
        }
        if (callbackContext.ReadValue<Vector2>() == answers[curIndex])
        {
            curIndex++;
            if (curIndex >= 11)
            {
                Cheat();
            }
        }
        else
        {
            curIndex = 0;
        }
    }

    void Cheat()
    {
        Debug.Log("We are so cheating in here");
        foreach (var item in ObjectsToSpawn)
        {
            Instantiate(item, transform.position, Quaternion.identity);
        }
    }
}
