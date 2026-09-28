using UnityEngine;

public class SoundCaller : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AudioMonolith.instance.PlaySound(0));
        StartCoroutine(AudioMonolith.instance.PlaySound(1));
        StartCoroutine(AudioMonolith.instance.PlaySound(2));
        StartCoroutine(AudioMonolith.instance.PlaySound(3));
        StartCoroutine(AudioMonolith.instance.PlaySound(4));
        StartCoroutine(AudioMonolith.instance.PlaySound(5));
        StartCoroutine(AudioMonolith.instance.PlaySound(6));
        StartCoroutine(AudioMonolith.instance.PlaySound(7));
        StartCoroutine(AudioMonolith.instance.PlaySound(8));
        StartCoroutine(AudioMonolith.instance.PlaySound(9));
        StartCoroutine(AudioMonolith.instance.PlaySound(10));
        StartCoroutine(AudioMonolith.instance.PlaySound(11));
        StartCoroutine(AudioMonolith.instance.PlaySound(12));
        StartCoroutine(AudioMonolith.instance.PlaySound(13));
        StartCoroutine(AudioMonolith.instance.PlaySound(14));
        StartCoroutine(AudioMonolith.instance.PlaySound(15));
        StartCoroutine(AudioMonolith.instance.PlaySound(16));
        StartCoroutine(AudioMonolith.instance.PlaySound(17));
        StartCoroutine(AudioMonolith.instance.PlaySound(18));
        StartCoroutine(AudioMonolith.instance.PlaySound(19));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

}
