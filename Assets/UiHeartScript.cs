using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class UiHeartScript : MonoBehaviour
{
    public List<SpriteRenderer> HeartStates = new List<SpriteRenderer>();
    public GameObject HpUp;
    
    Image image;
    
    int damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        HpUp.SetActive(false);
        
    }
    public void Damage(int damagelevel)
    {

        
        if(damage + damagelevel > HeartStates.Count-1)
        {
            Debug.Log("Do notin");
        }
        else
        {
            damage += damagelevel;
        }
            if (damage >= HeartStates.Count-1)
            {
                Debug.Log("Killyourself");
            }
        
       

    }
    public void Heal(int healing)
    {
        Debug.Log("heal");
        if (damage > 0)
        {
            if(damage - healing < 0)
            {
                damage = 0;
            }
            else
            {
                damage -= healing;
            }
                
           
            
            

           
        }
       



    }
    private void FixedUpdate()
    {
       
         image.sprite = HeartStates[damage].GetComponent<SpriteRenderer>().sprite;
        
    }
}
