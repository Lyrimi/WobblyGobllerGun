using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType {musket, bomber};
    [SerializeField] EnemyType enemyType;
    [SerializeField] float speed;
    [SerializeField] Transform target;
    [SerializeField] Transform playerTrans;

    [SerializeField] float debugnumba;

    private AIPath path;
    private float variableSpeed;

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

        if (enemyType == EnemyType.musket)
        {
            target.position = playerTrans.position + (this.transform.position - playerTrans.position).normalized * 5;
        }

    }
}
