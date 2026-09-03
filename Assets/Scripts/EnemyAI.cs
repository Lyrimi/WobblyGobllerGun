using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = UnityEngine.Random;

[RequireComponent(typeof(HealthSCript))]

public class EnemyAI : MonoBehaviour
{

    public enum EnemyType { musket, bomber, flintlock }; //what types of enemies are there, bomber unused
    [SerializeField] EnemyType enemyType; //what kind of enemy is it, used to change AI depending on whatever
    [SerializeField] float speed; //movement speed, used only to change maxspeed
    [SerializeField] Transform target; //where does the fella walk to
    [SerializeField] int dropChance; //weapon part drop chance
    [SerializeField] List<GameObject> drops = new List<GameObject>();



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
    }

    // Update is called once per frame
    void Update()
    {   //Is the player out of range, if they are, stop doing ANYTHING
        if (Vector3.Distance(transform.position, playerTrans.position) < 10)
        {
            if (outOfRange == true)
            {
                GetComponentInChildren<BulletSpawner>().shouldSpawn = true;
                path.maxSpeed = speed;
                outOfRange = false;
            }
            //then put back all the stuff when it works again^^^^

            //look at the player and walk to the target type shit
            path.destination = target.position;
            transform.up = playerTrans.position - this.transform.position;

            switch (enemyType)
            {
                case EnemyType.musket:    //Walk over to the player, stop when within range
                    target.position = playerTrans.position + (this.transform.position - playerTrans.position).normalized * 5;
                    break;
                //----------------------------------------------

                case EnemyType.flintlock: //walk around randomly,
                                          //shoot a ray to check if theres a wall where its walking to, try again if there is

                    if (Vector2.Distance(this.transform.position, target.transform.position) < 0.3)
                    {
                        Vector2 randomDir;
                        randomDir = Random.insideUnitCircle * 4;

                        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, randomDir, 4);

                        if (hit.collider == null)
                        {
                            target.position = transform.position + (Vector3)randomDir;
                        }
                    }                       //---------------------------------------------


                    break;
            }
        }
        //turn all the stuff off when out of range
        else
        {
            path.maxSpeed = 0;
            GetComponentInChildren<BulletSpawner>().shouldSpawn = false;
            outOfRange = true;
        }


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
        GameObject.Find("winCon").GetComponent<AreAllEnemiesDead>().enemyKills++;
        Destroy(this.gameObject);
    }
}
