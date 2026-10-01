using UnityEngine;

public class Explosion : MonoBehaviour
{
    float blowTimer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        blowTimer += Time.unscaledDeltaTime;

        if (blowTimer > 1f)
        {

            GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            GetComponent<Collider2D>().enabled = true;
        }

        if (blowTimer >= 1.32f)
        {
            Destroy(gameObject);
        }
    }
}
