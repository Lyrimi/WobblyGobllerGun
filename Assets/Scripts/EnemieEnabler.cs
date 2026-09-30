using System.Collections;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;

public class EnemieEnabler : MonoBehaviour
{
    [SerializeField] private Material _hitFlash;
    [SerializeField] private Material _disolve;
    [SerializeField] private float _spawnTime = 1.2f;
    private SpriteRenderer _spriteRend;
    private EnemyAI enemyAI;
    private AIPath aIPath;
    private BulletSpawner bulletSpawner;
    private HurtFlash hurtFlash;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRend = GetComponent<SpriteRenderer>();
        _spriteRend.material = _disolve;
        _spriteRend.material.SetFloat("_DisolveAmount", 1);
        enemyAI = GetComponent<EnemyAI>();
        aIPath = GetComponent<AIPath>();
        bulletSpawner = GetComponentInChildren<BulletSpawner>();
        hurtFlash = GetComponent<HurtFlash>();
        player = FindAnyObjectByType<PlayerMovementScript>(FindObjectsInactive.Include).gameObject;
        Disable();

        StartCoroutine(SpawnAnim());
    }

    void Disable()
    {
        enemyAI.enabled = false;
        aIPath.enabled = false;
        bulletSpawner.enabled = false;
    }

    IEnumerator SpawnAnim()
    {
        Material mat = _spriteRend.material;

        mat.SetFloat("_DisolveAmount", 1);

        float curTime = 0;

        while (curTime < _spawnTime)
        {
            transform.up = player.transform.position - transform.position;
            mat.SetFloat("_DisolveAmount", 1 - (curTime / _spawnTime));
            curTime += Time.deltaTime;
            yield return null;
        }
        mat.SetFloat("_DisolveAmount", 0);
        EnableEnemy();

    }

    void EnableEnemy()
    {
        _spriteRend.material = _hitFlash;
        hurtFlash._material = _spriteRend.material;

        enemyAI.enabled = true;
        aIPath.enabled = true;
        bulletSpawner.enabled = true;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
