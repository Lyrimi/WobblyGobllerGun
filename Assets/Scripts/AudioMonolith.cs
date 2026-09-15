
using UnityEngine;
using System.Collections.Generic;

public class AudioMonolith : MonoBehaviour
{
    
    public AudioSource AudioData;

    [SerializeField]
    int volume;
    

    public static AudioMonolith instance;

    public List<AudioClip> sources = new List<AudioClip>();

    void Awake()
    {
            instance = this;
            //print(this.gameObject.name);
        AudioData = GetComponent<AudioSource>();
        //Debug.Log($"{AudioData.name}");

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(int AudioIndex)
    {
        //Debug.Log($"{AudioIndex}");
        //Debug.Log($"{AudioData.name}");
        //Debug.Log($"{sources.Count}");
        AudioData.clip = sources[AudioIndex];
        AudioData.Play();
        if (AudioIndex !> sources.Count &&  sources.Count !< 0)
        {

            

        }
    }
}
