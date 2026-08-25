using UnityEngine;

public class SinusMove : MonoBehaviour
{
    private float cycle; // This variable increases with time and allows the sine to produce numbers between -1 and 1.
    private Vector3 basePosition; // This variable maintains the location of the object without applying sine changes
    private Vector3 startPosition;

    public Transform target;
    public Transform targeth;

    [SerializeField] float waveSpeed = 1f; // Higher make the wave faster
    [SerializeField] float bonusHeight = 1f; // Set higher if you want more wave intensity
    [SerializeField] float speed = 1f; // more value going faster to target


    public void Start()
    {
        startPosition = transform.position;
        targeth = GameObject.FindWithTag("Player").transform;
        target = targeth;
        transform.up = target.position - transform.position;
        basePosition = transform.position;
    }


    void Update()
    {
        cycle += Time.deltaTime * waveSpeed;

        transform.position = basePosition + (transform.right * bonusHeight) * Mathf.Sin(cycle);

        if (target) basePosition += (target.position - startPosition).normalized *Time.deltaTime * speed;
    }
}