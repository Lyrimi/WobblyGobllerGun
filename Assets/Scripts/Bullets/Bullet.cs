using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType { straight, wave, bomb };
    [SerializeField] BulletType bulletType;

    public float speed;
    public float lifetime;

    private float timer;

    Rigidbody2D rb;
    Transform player;

    #region Wave bullet variables
    private float cycle; // This variable increases with time and allows the sine to produce numbers between -1 and 1.
    private Vector3 basePosition; // This variable maintains the location of the object without applying sine changes
    private Vector3 startPosition;

    private Vector3 target;

    [Header("Wave bullet")]
    [SerializeField] float waveSpeed = 1f; // Higher make the wave faster
    [SerializeField] float bonusHeight = 1f; // Set higher if you want more wave intensity

    [Header("Bomb")]
    [SerializeField] GameObject explosionInstantiate;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        switch (bulletType)
        {
            case BulletType.straight:

                rb.linearVelocity = transform.right * speed;

                break;

            case BulletType.wave:

                startPosition = transform.position;
                target = transform.position + transform.up;
                transform.up = target - transform.position;
                basePosition = transform.position;

                break;

            case BulletType.bomb:

                player = GameObject.FindWithTag("Player").transform;

                float desiredFinalMagnitude = 1;
                Vector3 destination = new Vector3(player.position.x, player.position.y, 0);
                float distance = Vector3.Distance(destination, transform.position);
                float requiredSpeed = distance / desiredFinalMagnitude;

                float rand = Random.Range(0.8f, 1f);
                rb.linearDamping = rand;

                rb.linearVelocity = transform.right * requiredSpeed;

                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (timer > lifetime)
        {
            if (bulletType == BulletType.bomb)
            {
                Debug.Log("ploob");
                Instantiate(explosionInstantiate, gameObject.transform);
            }
            Destroy(this.gameObject);
        }

            timer += Time.deltaTime;

        switch (bulletType)
        {
            case BulletType.wave:
                cycle += Time.deltaTime * waveSpeed;
                transform.position = basePosition + (transform.right * bonusHeight) * Mathf.Sin(cycle);
                basePosition += (target - startPosition).normalized * Time.deltaTime * speed;
                break;
        }

    }
}
