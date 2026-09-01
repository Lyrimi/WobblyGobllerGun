using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType { straight, wave };
    [SerializeField] BulletType bulletType;

    public float speed;
    public float lifetime;

    private Vector2 spawnPoint;
    private float timer;

    #region Wave bullet variables
    private float cycle; // This variable increases with time and allows the sine to produce numbers between -1 and 1.
    private Vector3 basePosition; // This variable maintains the location of the object without applying sine changes
    private Vector3 startPosition;

    private Vector3 target;

    [Header("Wave bullet")]
    [SerializeField] float waveSpeed = 1f; // Higher make the wave faster
    [SerializeField] float bonusHeight = 1f; // Set higher if you want more wave intensity
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint = transform.position;

        if (bulletType == BulletType.wave)
        {
            startPosition = transform.position;
            target = transform.position+transform.up;
            transform.up = target - transform.position;
            basePosition = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > lifetime) Destroy(this.gameObject);
        timer += Time.deltaTime;

        switch (bulletType)
        {
            case BulletType.straight:
                transform.position = Movement(timer);
                break;

            case BulletType.wave:
                cycle += Time.deltaTime * waveSpeed;
                transform.position = basePosition + (transform.right * bonusHeight) * Mathf.Sin(cycle);
                basePosition += (target - startPosition).normalized * Time.deltaTime * speed;
                break;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.layer == )
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //collision.collider
    }

    private Vector2 Movement(float timer)
    {
                float x = timer * speed * transform.right.x;
                float y = timer * speed * transform.right.y;
                return new Vector2(x + spawnPoint.x, y + spawnPoint.y);
    }
}
