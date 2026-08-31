using Unity.VisualScripting;
using UnityEngine;

public class DamageScript : MonoBehaviour
{

    
    public int damageLevel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SendMessage("Damage", damageLevel);
            //GetComponent<HealthSCript>().Damage(damageLevel); 
           // 
       
    }
}
