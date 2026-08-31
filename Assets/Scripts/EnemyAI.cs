using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType {musket, bomber, flintlock};
    [SerializeField] EnemyType enemyType;
    [SerializeField] float speed;
    [SerializeField] Transform target;
    [SerializeField] Transform playerTrans;
    [SerializeField] float health;

    [SerializeField] float debugnumba;

    private AIPath path;
    BulletData bulletData;
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        path = GetComponent<AIPath>();
        playerTrans = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        path.maxSpeed = speed;
        path.destination = target.position;

        transform.up = playerTrans.position - this.transform.position;

        switch (enemyType)
        {
            case EnemyType.musket:
                target.position = playerTrans.position + (this.transform.position - playerTrans.position).normalized * 5;
                break;

            case EnemyType.flintlock:

                if(Vector2.Distance(this.transform.position, target.transform.position) < 0.3 )
                {
                    Vector2 randomDir;
                    randomDir = Random.insideUnitCircle * 4;
                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, randomDir, 4);
                    Debug.DrawRay(this.transform.position, randomDir, Color.red,2);
                    if (hit.collider == null)
                    {
                        target.position = transform.position + (Vector3)randomDir;
                    }
                }
                break;
        }
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
