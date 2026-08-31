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
    public void Damage()
    {

        if (damage < HeartStates.Count)
        {
            damage++;
            
            if (damage == HeartStates.Count-1)
            {
                Debug.Log("Killyourself");
            }
        }
        Debug.Log("damage");

    }
    public void Heal()
    {
        
        if (damage > 0)
        {
            damage--;
            HpUp.SetActive(true);
            var col = gameObject.GetComponent<SpriteRenderer>().material.color;
            

            for (int i = 100; i > 0; i--)
            {
                col.a = i/100;
                if (i <= 0)
                {
                    HpUp.SetActive(false);
                }
            }
           
        }
        Debug.Log("heal");



    }
    private void FixedUpdate()
    {
         image.sprite = HeartStates[damage].GetComponent<SpriteRenderer>().sprite;
    }
}
