using UnityEngine;

public class Explosion : MonoBehaviour
{
    float timer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer < 0.24 || timer > 1.32)
        {
            GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            GetComponent<Collider2D>().enabled = true;
        }

        if (timer >= 1.33 )
        {
            Destroy(gameObject);
        }
    }
}
