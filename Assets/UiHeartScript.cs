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

    int _curHealthLevel;
    int curHealthLevel
    {
        set
        {
            _curHealthLevel = value;
            UpdateSprite();
        }
        get
        {
            return _curHealthLevel;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        image = GetComponent<Image>();
        HpUp.SetActive(false);
    }

    //depricated
    public void Damage(int damagelevel)
    {
        throw new Exception("Depricade function use SetHealth");
        Debug.Log("Healthing it");

        if (curHealthLevel + damagelevel > HeartStates.Count - 1)
        {
            Debug.Log("Do notin");
        }
        else
        {
            curHealthLevel += damagelevel;
        }
        if (curHealthLevel >= HeartStates.Count - 1)
        {
            Debug.Log("Killyourself");
        }
    }

    public void SetHealth(int Health)
    {
        curHealthLevel = Health;
    }



    //depricated
    public void Heal(int healing)
    {
        throw new Exception("Depricade function use SetHealth");
        Debug.Log("heal");
        if (curHealthLevel > 0)
        {
            if (curHealthLevel - healing < 0)
            {
                curHealthLevel = 0;
            }
            else
            {
                curHealthLevel -= healing;
            }
        }
    }

    private void UpdateSprite()
    {
        image.sprite = HeartStates[curHealthLevel].GetComponent<SpriteRenderer>().sprite;
    }
    private void FixedUpdate()
    {



    }
}
