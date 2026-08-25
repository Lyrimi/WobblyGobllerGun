using UnityEngine;

public class TestingEditor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    public bool Spawn;
    // Update is called once per frame
    void Update()
    {
        if (Spawn)
        {
            Spawn = !Spawn;

        }
    }
}
