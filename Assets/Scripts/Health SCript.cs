using UnityEngine;

public class HealthSCript : MonoBehaviour
{

    public int health;
    [SerializeField]
    int MaxHealth;
    [SerializeField]
    bool IsPlayer;

    public UiHeartScript UiHeart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (UiHeart)
        {
            UiHeart.SetHealth(health);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Healing(int healingAmount)
    {
        if (health + healingAmount > MaxHealth)
        {
            health = MaxHealth;
        }
        else
        {
            health += healingAmount;

        }
        if (UiHeart)
        {
            UiHeart.SetHealth(health);
        }
    }


    public void Damage(int damageAmount)
    {

        if (health > 0)
        {
            health -= damageAmount;
        }
        if (UiHeart)
        {
            UiHeart.SetHealth(health);
        }
        if (health <= 0)
        {
            gameObject.SendMessage("Die");
        }

    }

}
