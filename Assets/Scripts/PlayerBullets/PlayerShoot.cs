using UnityEngine;
using UnityEngine.InputSystem;

class PlayerShoot : MonoBehaviour
{
    InputAction FireAction;
    float cooldownTime;

    void Start()
    {
        FireAction = InputSystem.actions.FindAction("Fire");
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
                GunData.bulletData.SpawnBullet();
                cooldownTime = GunData.FireRate;
            }
            else
            {
                cooldownTime -= Time.fixedDeltaTime;
            }
        }

    }


}