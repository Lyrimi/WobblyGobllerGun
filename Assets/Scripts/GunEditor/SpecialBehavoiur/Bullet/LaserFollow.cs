using UnityEngine;

public class LaserFollow : SpecialBulletBehavoiur
{
    Rigidbody2D rb;
    public override void OnFire()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public override void OnUpdate()
    {
        if (LaserTracking.Instance == null)
        {
            Debug.LogWarning("Laser Tracking Instance Is Missing");
            return;
        }

        transform.up = LaserTracking.Instance.transform.up;

        float velocity = rb.linearVelocity.magnitude;

        Vector2 fixedVelocity = transform.up * velocity;

        rb.linearVelocity = fixedVelocity;

    }
}
