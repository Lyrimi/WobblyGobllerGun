using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;


public class ParticleManager : MonoBehaviour
{
    
    public List<ParticleSystem> ParticleSystem = new List<ParticleSystem>();
    
    [SerializeField]
    bool particlesPositionFromObject;
    [SerializeField]
    bool particlesRotationFromObject;
    ParticleSystem.ShapeModule shapeModule;

    [SerializeField]
    float _startSpeed;

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (ParticleSystem p in ParticleSystem)
        {
            p.Stop();
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ParticleSystem p in ParticleSystem)
        {
            if (particlesPositionFromObject)
            {
                p.transform.position = gameObject.transform.localPosition;
            }
            if (particlesRotationFromObject)
            {
                p.transform.rotation = new Quaternion(gameObject.transform.rotation.x, 90, 0, 0);
            }
            var main = p.main;

            main.startSpeed = _startSpeed;
        }
        

        //shapeModule.rotation = new Vector3(gameObject.transform.rotation.x,90,0);

    }
    public IEnumerator PlayParticles(float waitTime,int Index)
    {
        Debug.Log("PlayedPArticles");
        ParticleSystem[Index].Play();
        yield return new WaitForSeconds(waitTime);
        ParticleSystem[Index].Stop();
        yield return null;

    }
   

}
