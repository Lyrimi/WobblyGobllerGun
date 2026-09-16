using UnityEngine;

public class SoundCaller : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioMonolith.instance.PlaySound(0);
        AudioMonolith.instance.PlaySound(1);
        AudioMonolith.instance.PlaySound(2);
        AudioMonolith.instance.PlaySound(5);
        AudioMonolith.instance.PlaySound(4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

}
