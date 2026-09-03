using UnityEngine;
using UnityEngine.InputSystem.Controls;

class BulletFlying : MonoBehaviour
{
    BulletData _bulletData;
    Rigidbody2D _rb;

    float _aliveTime = 0;
    void Start()
    {

    }

    public void Fire()
    {
        _rb = GetComponent<Rigidbody2D>();
        _bulletData = GunData.bulletData;
        _rb.linearVelocity = transform.up * _bulletData.Speed;
    }

    void FixedUpdate()
    {
        if (_aliveTime > _bulletData.LifeTime)
        {
            Destroy(gameObject);
            Destroy(this);
        }

        _aliveTime += Time.fixedDeltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.transform.CompareTag("Enemy"))
        {
            HealthSCript healthobj = collision.gameObject.GetComponent<HealthSCript>();
            if (!healthobj)
            {
                return;
            }
            Hit(healthobj);
        }
        else
        {
            HitWall();
        }
        //print($"Hit Something {collision.gameObject.name} | {Time.time}");
    }

    void HitWall()
    {
        Destroy(gameObject);
        Destroy(this);
    }

    void Hit(HealthSCript health)
    {

        health.Damage((int)_bulletData.Damage);
        Destroy(gameObject);
        Destroy(this);
    }
}