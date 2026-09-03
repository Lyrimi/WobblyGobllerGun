using System;
using Unity.VisualScripting;
using UnityEngine;

public class HealthSCript : MonoBehaviour
{

    public int health;
    [SerializeField]
    int MaxHealth;


    public GameObject HealthUp;

    public UiHeartScript UiHeart;

    Color TempColor;
    bool fade;
    [SerializeField]
    float IFrames;

    public float IFrameAmount;


    //Other scripts can subscribe to this
    public Action OnDeath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (UiHeart)
        {
            UiHeart.SetHealth(health);
        }
        TempColor = HealthUp.GetComponent<SpriteRenderer>().color;
        Debug.Log($"{HealthUp.GetComponent<SpriteRenderer>().color.a}");
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

        fade = true;
        TempColor.a = 1;
        HealthUp.SetActive(true);




    }
    public void FixedUpdate()
    {
        if (fade)
        {
            TempColor.a -= 0.01f;
            HealthUp.GetComponent<SpriteRenderer>().color = TempColor;
            if (TempColor.a < 0.02f)
            {
                fade = false;
                HealthUp.SetActive(false);
            }
        }
        if (IFrames > 0)
        {
            IFrames--;
        }

    }

    public void Damage(int damageAmount)
    {
        if (IFrames > 0)
        {
            return;
        }
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
            //Invokes the on death event
            OnDeath?.Invoke();

        }

        IFrames = IFrameAmount;


    }


}
