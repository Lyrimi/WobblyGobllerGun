using System;
using UnityEngine;

public class PuppyMurder : MonoBehaviour
{


    BulletData _bulletData;
    Rigidbody2D _rb;

    public Action<Collision2D> onHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
        //print($"Hit Something {collision.gameObject.name} | {Time.time}");
    }

    void Hit(HealthSCript health)
    {

        health.Damage((int)_bulletData.Damage);
    }

}
