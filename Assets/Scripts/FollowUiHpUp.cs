using UnityEngine;

public class FollowUiHpUp : MonoBehaviour
{

    Transform FollowTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FollowTarget = transform.parent;
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = FollowTarget.position;
        
    }
}
