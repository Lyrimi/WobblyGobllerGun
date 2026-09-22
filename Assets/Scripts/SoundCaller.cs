using UnityEngine;

public class SoundCaller : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AudioMonolith.instance.PlaySound(1));
        StartCoroutine(AudioMonolith.instance.PlaySound(2));
        StartCoroutine(AudioMonolith.instance.PlaySound(3));
        StartCoroutine(AudioMonolith.instance.PlaySound(4));
        StartCoroutine(AudioMonolith.instance.PlaySound(5));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

}
