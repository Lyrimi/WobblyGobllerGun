using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType { straight, wave };
    [SerializeField] BulletType bulletType;

    public float speed;
    public float lifetime;
    public float rotation;

    private Vector2 spawnPoint;
    private float timer;

    #region Wave bullet variables
    private float cycle; // This variable increases with time and allows the sine to produce numbers between -1 and 1.
    private Vector3 basePosition; // This variable maintains the location of the object without applying sine changes
    private Vector3 startPosition;

    public Transform target;
    public Transform targeth;

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
            targeth = GameObject.FindWithTag("Player").transform;
            target = targeth;
            transform.up = target.position - transform.position;
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
                if (target) basePosition += (target.position - startPosition).normalized * Time.deltaTime * speed;
                break;
        }
        
    }

    private Vector2 Movement(float timer)
    {
                float x = timer * speed * transform.right.x;
                float y = timer * speed * transform.right.y;
                return new Vector2(x + spawnPoint.x, y + spawnPoint.y);
    }
}
