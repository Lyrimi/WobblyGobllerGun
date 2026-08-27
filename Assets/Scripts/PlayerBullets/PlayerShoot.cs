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
                if (GunData.FireRate <= 0)
                {
                    return;
                }
                print("Bang");
                GunData.bulletData.ShootBullets();
                cooldownTime = 1f / GunData.FireRate;
            }

        }
        cooldownTime -= Time.fixedDeltaTime;

    }


}