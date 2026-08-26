using System.Collections;
using Pathfinding;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovementScript : MonoBehaviour
{
    Animator animator;
    
    public float speed;
    public float rotationSpeed;
    InputAction moveAction;
    Rigidbody2D rb;
    InputAction MousePosition;
    

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponentInChildren<Animator>();
    }

    /*void Move()
     {
         Vector2 move = moveAction.ReadValue<Vector2>();
     }
     */
    void Move()
    {
        Vector2 move = moveAction.ReadValue<Vector2>();
        
        rb.linearVelocity = move*speed;
        StartCoroutine("CheckMoving");
        
        

        /*
        rb.AddForce(speed * move);
        
        if (move.magnitude * speed  > rb.linearVelocity.magnitude)
        {
            Vector2 movedir = move.normalized;



            rb.AddForce( speed * movedir);
        }
       
        Vector2 rbvelocity = rb.linearVelocity;

        Vector2 corectedDirVel = rbvelocity.magnitude * transform.up;

        rb.linearVelocity = corectedDirVel;
        */
    }
    
    private IEnumerator CheckMoving()
    {
        Debug.Log("IEnum");
        Vector3 startPos = transform.position;
        yield return new ();
        Vector3 finalPos = transform.position;

        if (startPos.x != finalPos.x || startPos.y != finalPos.y
            || startPos.z != finalPos.z) { 
        
            Debug.Log("true");
            animator.SetBool("Moving", true);
        }
        else
        {
            Debug.Log("False");
            animator.SetBool("Moving", false);
        }
        
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
    void FixedUpdate()
    {
        Move();
        Rotate();
        

        
        
        
    }
    
    void Rotate()
    {


        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosWorld.z = 0;

        Vector3 lookDirection = mousePosWorld - transform.position;
        transform.up = Vector3.RotateTowards(transform.up , lookDirection, (rotationSpeed*Time.fixedDeltaTime)*Mathf.Deg2Rad,0);
        
        

        
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        
       
    }
}
