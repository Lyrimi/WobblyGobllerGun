using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType {musket, bomber, flintlock};
    [SerializeField] EnemyType enemyType;
    [SerializeField] float speed; //movement speed, used only to change maxspeed
    [SerializeField] Transform target;
    [SerializeField] Transform playerTrans;
    [SerializeField] float health;


    private AIPath path;
    BulletData bulletData;
    private bool outOfRange;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        path = GetComponent<AIPath>();
        playerTrans = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, playerTrans.position) < 10)
        {
            if (outOfRange == true)
            {
                GetComponentInChildren<BulletSpawner>().enabled = true;
                path.maxSpeed = speed;
                outOfRange = false;
            }

            path.maxSpeed = speed;
            path.destination = target.position;

            transform.up = playerTrans.position - this.transform.position;

            switch (enemyType)
            {
                case EnemyType.musket:
                    target.position = playerTrans.position + (this.transform.position - playerTrans.position).normalized * 5;
                    break;

                case EnemyType.flintlock:

                    if (Vector2.Distance(this.transform.position, target.transform.position) < 0.3)
                    {
                        Vector2 randomDir;
                        randomDir = Random.insideUnitCircle * 4;
                        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, randomDir, 4);
                        Debug.DrawRay(this.transform.position, randomDir, Color.red, 2);
                        if (hit.collider == null)
                        {
                            target.position = transform.position + (Vector3)randomDir;
                        }
                    }
                    break;
            }
        }
        else { path.maxSpeed = 0; }
        GetComponentInChildren<BulletSpawner>().enabled = false;
        outOfRange = true;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            health -= GunData.bulletData.Damage;
            Destroy(collision.gameObject);
            if (health<=0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
