using UnityEngine;

public class HealthSCript : MonoBehaviour
{

    public int health;

    public GameObject UiHeart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Healing(int healingamount)
    {
        if (gameObject.CompareTag("Player"))
        {
            if (health + healingamount > 3)
            {
                health = 3;
            }
            else
            {
                health += healingamount;
                
            }
            UiHeart.SendMessage("Heal", healingamount);
        }  
    }

    
    public void Damage(int damagelevel)
    {
        if (gameObject.CompareTag("Player"))
        {
            UiHeart.SendMessage("Health", damagelevel);
        }
        if (health > 0)
        {
            health -= damagelevel;
        }
        if (health <= 0)
        {
            gameObject.SendMessage("Die");
        }
    }

}
