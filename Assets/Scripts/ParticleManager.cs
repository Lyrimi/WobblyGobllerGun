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

    HealthSCript healthSCript;

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthSCript = GetComponent<HealthSCript>();
        foreach (ParticleSystem p in ParticleSystem)
        {
            p.Stop();
        }
        healthSCript.OnHit += StartParticles;
       
    }

   public void StartParticles()
    {
        ParticleSystem[2].Play();
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
           
        }
        

        //shapeModule.rotation = new Vector3(gameObject.transform.rotation.x,90,0);

    }
    public IEnumerator PlayParticles(float waitTime,int Index)
    {
        
        ParticleSystem[Index].Play();
        Debug.Log("PlayedPArticles");
        yield return new WaitForSeconds(waitTime);
        ParticleSystem[Index].Stop();
        yield return null;

    }
   

}
