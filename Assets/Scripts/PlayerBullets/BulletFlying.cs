using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering.Universal;

public class BulletFlying : MonoBehaviour
{
    BulletData _bulletData;
    Rigidbody2D _rb;

    float _aliveTime = 0;

    public Action onFire;
    public Action onUpdate;
    public Action<Collision2D> onHit;
    public Action onDestroy;

    void Start()
    {

    }

    public void Fire()
    {
        _rb = GetComponent<Rigidbody2D>();
        _bulletData = GunData.bulletData;
        _rb.linearVelocity = transform.up * _bulletData.Speed;
        onFire?.Invoke();
    }

    void FixedUpdate()
    {
        onUpdate?.Invoke();
        if (_aliveTime > _bulletData.LifeTime)
        {
            Destroy(gameObject);
            Destroy(this);
        }

        _aliveTime += Time.fixedDeltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        onHit?.Invoke(collision);
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
            Debug.Log(collision.gameObject.tag);
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

    void OnDestroy()
    {
        onDestroy?.Invoke();
    }
}