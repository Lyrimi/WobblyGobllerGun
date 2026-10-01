
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AudioMonolith : MonoBehaviour
{



    [SerializeField]
    [Range(0f, 1f)]
    float GlobalVolume;

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

        instance = this;

        dictionary.Add("C", 1);

    }


    public IEnumerator PlaySound(AudioClip audioClip, float localVolume = 1)
    {
        AudioSource curAudio = gameObject.AddComponent<AudioSource>();

        curAudio.PlayOneShot(audioClip, GlobalVolume * localVolume);

        yield return new WaitUntil(() => !curAudio.isPlaying);
    }

    public IEnumerator PlaySound(int AudioIndex, float localVolume = 1)
    {

        AudioSource curAudio;



        //Debug.Log($"Audiosources amount:{AudioSources.Count}");
        int i = AudioSources.Count - 1;
        //Debug.Log($"i: {i}");

        int i = AudioSources.Count - 1;

        curAudio = gameObject.AddComponent<AudioSource>();

        curAudio.PlayOneShot(sources[AudioIndex], GlobalVolume * localVolume);

        yield return new WaitUntil(() => !curAudio.isPlaying);
        Destroy(curAudio);
        //AudioSources.Remove(curAudio);







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
    
}
