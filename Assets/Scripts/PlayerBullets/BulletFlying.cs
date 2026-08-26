using UnityEngine;
using UnityEngine.InputSystem.Controls;

class BulletFlying : MonoBehaviour
{
    BulletData bulletData;
    Rigidbody2D rb;

    float AliveTime = 0;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.up * bulletData.Speed;
    }

    void FixedUpdate()
    {
        if (AliveTime > bulletData.LifeTime)
        {
            Destroy(gameObject);
            Destroy(this);
        }

        AliveTime += Time.fixedDeltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        print($"Hit Something {collision.gameObject.name} | {Time.time}");
    }
}