using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{

    
    public int healAmount;
    [SerializeField] bool spawnCorpse;
    [SerializeField] bool spawnBlood;

    [SerializeField] GameObject corpse;
    [SerializeField] new List<GameObject> bloodyMesses = new List<GameObject>();
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
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.SendMessage("Healing", healAmount);

            if (spawnCorpse)
            {
                Instantiate(corpse, transform.position, Quaternion.identity);
            }

            if (spawnBlood)
            {
                int randchance = Random.Range(0, bloodyMesses.Count);
                Instantiate(bloodyMesses[randchance], new Vector3(transform.position.x,transform.position.y,transform.position.z-1), Quaternion.identity);
            }
            Destroy(gameObject);
        }
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
