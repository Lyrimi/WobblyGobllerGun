using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(HealthSCript))]

public class EnemyAI : MonoBehaviour
{

    public enum EnemyType { musket, bomber, flintlock, puppy }; //what types of enemies are there, bomber unused
    [SerializeField] EnemyType enemyType; //what kind of enemy is it, used to change AI depending on whatever
    [SerializeField] float speed; //movement speed, used only to change maxspeed
    [SerializeField] Transform target; //where does the fella walk to
    [SerializeField] int dropChance; //weapon part drop chance
    [SerializeField] List<GameObject> drops = new List<GameObject>();

    [SerializeField] LayerMask mask;




    private AIPath path; //im not really sure to be honest, think its just where my boy walks
    private bool outOfRange; //used to check if the enemy is out of the players range, and should stop AI-ing
    Transform playerTrans; //wheres the player, used to find out where to put target and other things


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        path = GetComponent<AIPath>();
        playerTrans = GameObject.FindWithTag("Player").transform;
        HealthSCript healthScript = GetComponent<HealthSCript>();
        healthScript.OnDeath += Die;
        Vector3 nPos = transform.position;
        nPos.z = playerTrans.position.z;
        transform.position = nPos;
        path.maxSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {   //Is the player out of range, if they are, stop doing ANYTHING
        if (playerTrans == null)
        {
            return;
        }
/*
        if (GetComponent<SpriteRenderer>().isVisible == true)
        {
            if (outOfRange == true)
            {
                GetComponentInChildren<BulletSpawner>().shouldSpawn = true;
                GetComponent<HealthSCript>().enabled = true;
                path.maxSpeed = speed;
                outOfRange = false;
            }
*/
            //then put back all the stuff when it works again^^^^

            //look at the player and walk to the target type shit
            path.destination = target.position;
            if (enemyType != EnemyType.puppy)
            {
                transform.up = playerTrans.position - this.transform.position;
            }

            switch (enemyType)
            {
                case EnemyType.musket:
                    //I gave it a scope to avoid local variable conflicts
                    {
                        //Walk over to the player, stop when within range


                        //If theres is a wall between the player and the this, it walks directly to the player
                        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, (playerTrans.position - transform.position).normalized, (transform.position - target.position).magnitude, mask);
                        Debug.DrawLine(this.transform.position, transform.position + (playerTrans.position - transform.position), Color.red);
                        if (hit.collider != null)
                        {
                            target.position = playerTrans.position;
                        }
                        else
                        {
                            target.position = playerTrans.position + (this.transform.position - playerTrans.position).normalized * 5;
                        }
                        break;
                    }
                //----------------------------------------------

                case EnemyType.flintlock: //walk around randomly,
                                          //shoot a ray to check if theres a wall where its walking to, try again if there is
                    {
                        if (Vector2.Distance(this.transform.position, target.transform.position) < 0.3)
                        {
                            Vector2 randomDir;
                            randomDir = Random.insideUnitCircle * 4;

                            RaycastHit2D hit = Physics2D.Raycast(this.transform.position, randomDir, 4, mask);
                            Debug.DrawRay(this.transform.position, randomDir, Color.red);

                            if (hit.collider == null)
                            {
                                target.position = transform.position + (Vector3)randomDir;
                            }
                        }
                        break;
                    }
                //---------------------------------------------

                case EnemyType.puppy:
                    {
                        if (Vector2.Distance(this.transform.position, target.transform.position) < 2)
                        {
                            transform.up = playerTrans.position - this.transform.position;
                            RaycastHit2D hit = Physics2D.Raycast(this.transform.position, playerTrans.position - this.transform.position, 10, mask);
                            Debug.DrawRay(this.transform.position, playerTrans.position - this.transform.position, Color.red, 100);

                            if (hit == false)
                            {
                                target.position = (playerTrans.position - transform.position) + (playerTrans.position - transform.position).normalized * 10 + transform.position;
                            }
                            else
                            {
                                target.position = hit.point;
                            }

                        }

                        break;
                    }
                //---------------------------------------------

                case EnemyType.bomber:
                    {

                    }

                    break;
            }
        //}
    
        //turn all the stuff off when out of range
        /*
        if (!GetComponent<SpriteRenderer>().isVisible)
        {
            GetComponentInChildren<BulletSpawner>().shouldSpawn = false;
            path.maxSpeed = 0;
            GetComponent<HealthSCript>().enabled = false;
            outOfRange = true;
   
        }*/
    


    }

    //of you get hit take damage and ATOMIZE the bullet
    public void Die()
    {
        int rand = Random.Range(1, 100);
        if (rand <= dropChance)
        {
            int randchance = Random.Range(0, drops.Count);
            Instantiate(drops[randchance], transform.position, quaternion.identity);
        }
        //GameObject.Find("winCon").GetComponent<AreAllEnemiesDead>().enemyKills++;
        Destroy(this.gameObject);
    }
}
