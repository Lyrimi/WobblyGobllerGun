
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class AudioMonolith : MonoBehaviour
{
    
    AudioSource AudioData;

    [SerializeField]
    int volume;

    public Dictionary<string, int> dictionary = new Dictionary<string, int>();
    



    public static AudioMonolith instance;
    
    public List<AudioClip> sources = new List<AudioClip>();
    public List<AudioSource> AudioSources = new List<AudioSource>();
    void Awake()
    {
            instance = this;
        //print(this.gameObject.name);
        //AudioData = GetComponent<AudioSource>();
        //Debug.Log($"{AudioData.name}");


        dictionary.Add("C",1);


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
        //Debug.Log($"Name of the audio clip at index{sources[AudioIndex].name}");
        //Debug.Log($"AudioIndex{AudioIndex}");
        //Debug.Log($"{AudioIndex}");
        //Debug.Log($"{AudioData.name}");
        //Debug.Log($"{sources.Count}");

        AudioSources.Add(new AudioSource());
        
        

        //Debug.Log($"Audiosources amount:{AudioSources.Count}");
        int i = AudioSources.Count-1;
        //Debug.Log($"i: {i}");

        AudioSources[i] = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

        //AudioSources[i].clip = sources[AudioIndex];
        //Debug.Log($"{AudioSources[i].name}");
        AudioSources[i].PlayOneShot(sources[AudioIndex]);



        //Debug.Log($"{AudioSources[i].clip}");

        //AudioSources[i].clip = sources[AudioIndex];
        //Debug.Log($"AudioSources[i].clip = {AudioSources[i].clip}");
        //AudioSources[i-1].GetComponent<AudioSource>();
        //AudioSources[i].PlayOneShot(sources[AudioIndex]);
        //AudioSources[i].clip = sources[AudioIndex];
        //AudioSources[i].Play();
        //AudioData.PlayOneShot(sources[AudioIndex]);




        
    }
}
