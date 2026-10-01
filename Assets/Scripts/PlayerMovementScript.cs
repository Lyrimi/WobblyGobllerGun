using System.Collections;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(GunMenuEnabler))]
public class PlayerMovementScript : MonoBehaviour
{
    Animator animator;

    HealthSCript PlayerHealth;

    ParticleManager ParticleManager;

    [SerializeField]
    private float _flashTime;

    HurtFlash DashFlash;

    public float speed;
    public float rotationSpeed;
    private float startRotationSpeed;
    InputAction moveAction;
    InputAction Dash;
    Rigidbody2D rb;
    InputAction MousePosition;
    [SerializeField] LayerMask mask;
    LayerMask startmask;

    UiHeartScript HeartScript;
    [SerializeField]
    bool dashing;
    [SerializeField]
    bool canDash;
    [SerializeField]
    float dashTime;

    public float dashCooldown;

    Color gameobjectColor;

    BoxCollider2D boxCollider;

    private GunMenuEnabler gunMenuEnabler;

    private void Start()
    {
        PlayerHealth = GetComponent<HealthSCript>();
        DashFlash = GetComponent<HurtFlash>();
        moveAction = InputSystem.actions.FindAction("Move");
        Dash = InputSystem.actions.FindAction("Sprint");
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        ParticleManager = GetComponent<ParticleManager>();
        animator = GetComponentInChildren<Animator>();
        gunMenuEnabler = GetComponent<GunMenuEnabler>();

        HealthSCript healthScript = GetComponent<HealthSCript>();
        healthScript.OnDeath += Die;
        Dash.performed += Sprint;

        dashing = false;

        startmask = boxCollider.excludeLayers;
        startRotationSpeed = rotationSpeed;

        gunMenuEnabler.OnGunRebuild += RecalcData;

    }

    void RecalcData(bool passed)
    {
        if (passed)
        {
            rotationSpeed = startRotationSpeed * GunData.TurnSpeedMult;
        }
        else
        {
            rotationSpeed = startRotationSpeed;
        }

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
    void Sprint(InputAction.CallbackContext context)
    {
        Debug.Log("Dash");
        if (canDash)
        {
            dashing = true;
        }
        else return;

        boxCollider.excludeLayers = mask;


        StartCoroutine(Wait(dashTime, dashCooldown));
        if (dashing && canDash && rb.linearVelocity != Vector2.zero)
        {
            DashFlash.StartCoroutine(DashFlash.Flash());
            ParticleManager.StartCoroutine(ParticleManager.PlayParticles(dashTime, 0));

            PlayerHealth.IFrames = 10;

            rb.linearVelocity *= 5;

            canDash = false;

            DashFlash.StartCoroutine(DashFlash.ReverseFlash());
            

        }
    }

    private IEnumerator Wait(float waitTime, float dashdelay)
    {

        yield return new WaitForSeconds(waitTime);
        dashing = false;
        boxCollider.excludeLayers = startmask;
        yield return new WaitForSeconds(dashdelay);
        canDash = true;



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
        if (!dashing)
        {
            Move();
            Rotate();
        }






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
