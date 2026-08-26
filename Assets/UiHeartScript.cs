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
       
    }
    public void Damage()
    {
        
        if (damage <= HeartStates.Count)
        {
            image.sprite = HeartStates[damage++];
        }
        //else (Killyourself)
        
    }
    // Update is called once per frame

    
}
