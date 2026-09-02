using UnityEngine;

public class TestSciptGen : MonoBehaviour
{
    public BoxCollider2D Test1;
    public BoxCollider2D Test2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Test1.bounds.Intersects(Test2.bounds))
        {
            print("Is Touching ");
        }
    }
}
