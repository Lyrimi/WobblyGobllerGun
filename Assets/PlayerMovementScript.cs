using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    public float speed;
    InputAction moveAction;
    Rigidbody2D rb;
    InputAction MousePosition;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        
        rb = GetComponent<Rigidbody2D>();
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
        //rb.AddForce(speed * move);
        /*
        if (move.magnitude * speed  > rb.linearVelocity.magnitude)
        {
            Vector2 movedir = move.normalized;



            rb.AddForce( speed * movedir);
        }
        */

        /*
        Vector2 rbvelocity = rb.linearVelocity;

        Vector2 corectedDirVel = rbvelocity.magnitude * transform.up;

        rb.linearVelocity = corectedDirVel;

        */
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
        rb.angularVelocity = 0;

        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosWorld.z = 0;
        Vector3 lookDirection = mousePosWorld - transform.position;

        transform.up = lookDirection.normalized;
        
        
    }
    private void Update()
    {
        
       
    }

}
