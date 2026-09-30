using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthSCript))]
public class HurtFlash : MonoBehaviour
{ //_FlashAmount
    [SerializeField] private float _flashTime;
    private Material _material;
    private HealthSCript _healthSCript;
    private PlayerMovementScript _movementScript;
    [SerializeField]
    float refreshGlow;
    [SerializeField]
    float refreshGlowIntensity;
    ParticleManager _particles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _healthSCript = GetComponent<HealthSCript>();
        _healthSCript.OnHit += StartFlash;
        _material = GetComponentInChildren<SpriteRenderer>().material;
        _movementScript = GetComponent<PlayerMovementScript>();
        _particles = GetComponentInChildren<ParticleManager>(); 
    }


    void StartFlash()
    {
        StopAllCoroutines();
        StartCoroutine(Flash());
    }

    public IEnumerator Flash()
    {
        float curFlash = _flashTime;

        while (curFlash > 0)
        {
            _material.SetFloat("_FlashAmount", curFlash / _flashTime);
            curFlash -= Time.deltaTime;
            yield return null;
        }
        _material.SetFloat("_FlashAmount", 0);
    }
    public IEnumerator ReverseFlash()
    {

        yield return new WaitForSeconds(_movementScript.dashCooldown);
        _material.SetColor("_FlashColor", Color.yellowNice);
        _particles.StartCoroutine(_particles.PlayParticles(0, 1));
        Debug.Log("ReverseFlashing");
        float curFlash = 0;
        
        while (curFlash < refreshGlow)
        {
           
            _material.SetFloat("_FlashAmount", curFlash + _flashTime*refreshGlowIntensity);
            
            curFlash += Time.deltaTime;
           
            yield return new Color();
           
        }
        _material.SetColor("_FlashColor", Color.white);
        _material.SetFloat("_FlashAmount", 0);
    }
}
