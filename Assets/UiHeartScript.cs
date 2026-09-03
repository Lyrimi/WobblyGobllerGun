using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiHeartScript : MonoBehaviour
{
    public List<SpriteRenderer> HeartStates = new List<SpriteRenderer>();
    public GameObject HpUp;
    [SerializeField]
    Animator Animor;

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
    void Start()
    {
        image = GetComponent<Image>();
        HpUp.SetActive(false);
        //Animor = GetComponent<Animator>();

    }

    //depricated
    public void Damage(int damagelevel)
    {
        throw new Exception("Depricade function use SetHealth");

    }

    public void SetHealth(int Health)
    {
        curHealthLevel = Health;
    }



    //depricated
    public void Heal(int healing)
    {
        throw new Exception("Depricade function use SetHealth");

    }



    void UpdateSprite()
    {
        if (Animor == null)
        {
            Debug.LogWarning("No Animor Found");
        }
        Animor.SetInteger("HealthUiInt", _curHealthLevel);
        // Animor.Play(0);
    }

    /* private void FixedUpdate()
     {

         image.sprite = HeartStates[curHealthLevel].GetComponent<SpriteRenderer>().sprite;

     }
     */
}
