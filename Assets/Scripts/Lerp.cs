using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Lerp : MonoBehaviour
{

    GameObject current;
    [SerializeField] GameObject target;
    [SerializeField] float strength;
    [SerializeField] bool gotransform;
    [SerializeField] bool gorotate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        current = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (gotransform == true)
        {
            if ((transform.position - target.transform.position).magnitude >= 0.07f)
            {
                current.transform.position = Vector3.Lerp(current.transform.position, new Vector3(target.transform.position.x, target.transform.position.y, current.transform.position.z), (Time.deltaTime * strength));
            }
            else
            {
                current.transform.position = target.transform.position;
                GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            }
        }

        if (gorotate == true)
        {
            if (Quaternion.Angle(current.transform.rotation, target.transform.rotation) >= 3)
            {
                current.transform.rotation = Quaternion.Lerp(current.transform.rotation, target.transform.rotation, (Time.deltaTime * 12));
            }
            else
            {
                current.transform.rotation = target.transform.rotation;
            }
        }
    }
}
