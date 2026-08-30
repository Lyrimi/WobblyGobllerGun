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
        image.sprite = HeartStates[damage];
    }
    public void Damage()
    {

        if (damage < HeartStates.Count)
        {
            damage++;
            image.sprite = HeartStates[damage];
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
            image.sprite = HeartStates[damage];
           
        }
        Debug.Log("heal");



    }
}
