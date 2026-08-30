using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UiHeartScript : MonoBehaviour
{
    public List<Sprite> HeartStates = new List<Sprite>();
    
    public Image image;

    int damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        image.sprite = HeartStates[damage++];
    }
    public void Damage()
    {
        
        if (damage < HeartStates.Count)
        {
            image.sprite = HeartStates[damage++];
            if (damage == HeartStates.Count) 
            {
             Debug.Log("Killyourself");
            }
        }
        
        
    }
    

    
}
