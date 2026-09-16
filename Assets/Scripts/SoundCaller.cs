using UnityEngine;

public class SoundCaller : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioMonolith.instance.PlaySound(11);
        AudioMonolith.instance.PlaySound(10);
        AudioMonolith.instance.PlaySound(9);
        AudioMonolith.instance.PlaySound(8);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

}
