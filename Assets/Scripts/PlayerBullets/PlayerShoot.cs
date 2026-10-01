using UnityEngine;
using UnityEngine.InputSystem;

class PlayerShoot : MonoBehaviour
{
    InputAction FireAction;
    float cooldownTime;
    TopDownGunData topDownGunData;

    void Start()
    {
        FireAction = InputSystem.actions.FindAction("Fire");
        topDownGunData = GetComponent<TopDownGunData>();
        muzzelFlash = topDownGunData.EndPoints[0].GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (FireAction.IsPressed())
        {
            if (cooldownTime <= 0)
            {
                if (GunData.bulletData == null)
                {
                    return;
                }
                if (GunData.FireRate <= 0)
                {
                    return;
                }
                print("Bang");
                GunData.bulletData.ShootBullets();
                MuzzelFlash();
                cooldownTime = 1f / GunData.FireRate;
            }

        }
        cooldownTime -= Time.fixedDeltaTime;

    }

    private SpriteRenderer muzzelFlash;
    [SerializeField]
    private float flashTime = 0.02f;
    void MuzzelFlash()
    {
        if (GunData.audioClip != null)
        {
            StartCoroutine(AudioMonolith.instance.PlaySound(GunData.audioClip, GunData.AudioVolume));
        }

        if (muzzelFlash == null)
        {
            return;
        }

        muzzelFlash.enabled = true;

        Invoke("DisableMuzzelFlash", flashTime);
    }

    void DisableMuzzelFlash()
    {
        muzzelFlash.enabled = false;
    }
}
