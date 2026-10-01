using UnityEngine;

public class SplitBullet : SpecialBulletBehavoiur
{

    private float _curTime;
    private float _splitTime = 0.2f;
    private BulletData _copyBullet = new();
    const float VOLUME = 0.75f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnFire()
    {
        _copyBullet.Damage = GunData.bulletData.Damage / 3f;
        _copyBullet.LifeTime = GunData.bulletData.LifeTime / 2f;
        _copyBullet.Spreed = 45f;
        _copyBullet.Speed = GunData.bulletData.Speed / 1.5f;
        _copyBullet.Projectiles = 5;

        _curTime = 0f;
    }

    public override void OnUpdate()
    {
        _curTime += Time.fixedDeltaTime;
        if (_curTime > _splitTime)
        {
            Split();
        }
    }

    void Split()
    {
        StartCoroutine(AudioMonolith.instance.PlaySound(28, VOLUME));
        _copyBullet.OverideSpawnPoints = new Transform[] { transform };
        _copyBullet.ShootBullets();
        Destroy(gameObject);
        Destroy(this);
    }
}