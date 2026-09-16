
using UnityEngine;
using System.Collections.Generic;

public class AudioMonolith : MonoBehaviour
{
    
    AudioSource AudioData;

    [SerializeField]
    int volume;
    

    public static AudioMonolith instance;

    public List<AudioClip> sources = new List<AudioClip>();
    public List<AudioSource> AudioSources = new List<AudioSource>();
    void Awake()
    {
            instance = this;
            //print(this.gameObject.name);
        //AudioData = GetComponent<AudioSource>();
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
        
        AudioSources.Add(new AudioSource());
        AudioSources[AudioIndex].Play();
        
        if (AudioIndex !> sources.Count &&  sources.Count !< 0)
        {

            

        }
    }
}
