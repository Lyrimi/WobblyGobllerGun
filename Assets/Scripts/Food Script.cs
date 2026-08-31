using UnityEngine;

public class FoodScript : MonoBehaviour
{

    public GameObject obj;
    public int healAmount;
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
        obj.SendMessage("Heal",healAmount);
    }
    /*
    if (other.gameObject.CompareTag("Bullet"))
        {
            HeartScript.Damage();
            Destroy(other.gameObject);
        }

    Debug.Log("Colliding it");

        if (other.gameObject.CompareTag("Food"))
        {
            HeartScript.Heal();
            Destroy(other.gameObject);
    
        }
    */
}
