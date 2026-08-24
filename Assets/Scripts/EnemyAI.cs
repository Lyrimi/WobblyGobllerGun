using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType {musket, bomber};
    [SerializeField] EnemyType enemyType;
    [SerializeField] Transform target;
    [SerializeField] Transform playerTrans;

    NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.up = target.position - this.transform.position;
        agent.SetDestination(target.position);

        if (enemyType == EnemyType.musket)
        {
            target.position = playerTrans.position;
        }

    }
}
