using System.Collections;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovementScript : MonoBehaviour
{
    Animator animator;

    public float speed;
    public float rotationSpeed;
    InputAction moveAction;
    Rigidbody2D rb;
    InputAction MousePosition;

    UiHeartScript HeartScript;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");

        rb = GetComponent<Rigidbody2D>();

        animator = GetComponentInChildren<Animator>();

        HealthSCript healthScript = GetComponent<HealthSCript>();
        healthScript.OnDeath += Die;
    }


    void Move()
    {
        Vector2 move = moveAction.ReadValue<Vector2>();

        rb.linearVelocity = move * speed;

        //StartCoroutine("CheckMoving");
        if (rb.linearVelocity != Vector2.zero)
        {
            animator.SetBool("Moving", true);
        }
        else
        {
            animator.SetBool("Moving", false);
        }



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
        Vector2 startPos = transform.position;
        yield return new();
        Vector2 finalPos = transform.position;

        if (startPos != finalPos)
        {
            animator.SetBool("Moving", true);
        }
        else
        {
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
        transform.up = Vector3.RotateTowards(transform.up, lookDirection, (rotationSpeed * Time.fixedDeltaTime) * Mathf.Deg2Rad, 0);





    }

    void OnCollisionEnter2D(Collision2D collision)
    {



    }
    private void OnTriggerEnter2D(Collider2D other)
    {

    }

    public void Die()
    {
        SceneManager.LoadScene("Menui scene");
    }

}
