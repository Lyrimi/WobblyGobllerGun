using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = UnityEngine.Random;

[RequireComponent(typeof(HealthSCript))]

public class EvilSphere : MonoBehaviour
{

    [SerializeField] float speed; //movement speed, used only to change maxspeed
    [SerializeField] Transform target; //where does the fella walk to
    [SerializeField] List<GameObject> attacks = new List<GameObject>();
    [SerializeField] float attackCooldown; //how long between attacks
    [SerializeField] LayerMask mask;


    private AIPath path; //im not really sure to be honest, think its just where my boy walks
    private bool outOfRange; //used to check if the enemy is out of the players range, and should stop AI-ing
    Transform playerTrans; //wheres the player, used to find out where to put target and other things
    private float attackTimer; //timer for attacks
    private bool attacking;
    private int rand;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        path = GetComponent<AIPath>();
        playerTrans = GameObject.FindWithTag("Player").transform;
        HealthSCript healthScript = GetComponent<HealthSCript>();
        healthScript.OnDeath += Die;
    }

    // Update is called once per frame
    void Update()
    {   //Is the player out of range, if they are, stop doing ANYTHING
        if (playerTrans == null)
        {
            return;
        }


        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown && !attacking)
        {
            rand = Random.Range(0, attacks.Count);
            attacks[rand].GetComponent<BulletSpawner>().enabled = true;
            attacking = true;
        }

        if (attacking)
        {
            if (attacks[rand].GetComponent<BulletSpawner>().finishedAttacking)
            {
                attacks[rand].GetComponent<BulletSpawner>().finishedAttacking = false;
                attacks[rand].GetComponent<BulletSpawner>().enabled = false;
                attacking = false;
                attackTimer = 0;
            }
        }
        

        if (GetComponent<MeshRenderer>().isVisible == true)
        {
            Debug.Log("poob");
            if (outOfRange == true)
            {
                Debug.Log("not out of range");
                GetComponent<HealthSCript>().enabled = true;
                path.maxSpeed = speed;
                outOfRange = false;
            }
            
             transform.up = playerTrans.position - this.transform.position;

            Debug.Log(Vector2.Distance(this.transform.position, target.transform.position));

            if (Vector2.Distance(this.transform.position, target.transform.position) < 0.3)
            {
                Debug.Log("moving!!");
                Vector2 randomDir;
                randomDir = Random.insideUnitCircle * 10;

                RaycastHit2D hit = Physics2D.Raycast(this.transform.position, randomDir, 10);
                Debug.DrawRay(this.transform.position, randomDir, Color.red);

                if (hit.collider == null)
                {
                    target.position = transform.position + (Vector3)randomDir;
                }
            }

            path.destination = target.position;
        }
        //turn all the stuff off when out of range
        if (!GetComponent<MeshRenderer>().isVisible)
        {
            Debug.Log("out of range");
            path.maxSpeed = 0;
            GetComponent<HealthSCript>().enabled = false;
            outOfRange = true;
        }


    }

    //of you get hit take damage and ATOMIZE the bullet
    public void Die()
    {
        Destroy(this.gameObject);
    }
}
