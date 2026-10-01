
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AudioMonolith : MonoBehaviour
{
    


    [SerializeField]
    int volume;

    public Dictionary<string, int> dictionary = new Dictionary<string, int>();

    public static AudioMonolith instance;
    
    public List<AudioClip> sources = new List<AudioClip>();
    public List<AudioSource> AudioSources = new List<AudioSource>();


    void Awake()
    {
            instance = this;

        dictionary.Add("C",1);

    }


    public IEnumerator PlaySound(int AudioIndex)
    {

        AudioSource curAudio;

        int i = AudioSources.Count-1;

        curAudio = gameObject.AddComponent<AudioSource>();

        curAudio.PlayOneShot(sources[AudioIndex]);
        
        yield return new WaitUntil(() => !curAudio.isPlaying);
        Destroy(curAudio);
    }
    
}
