using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthSCript))]
public class HurtFlash : MonoBehaviour
{ //_FlashAmount
    [SerializeField] private float _flashTime;
    private Material _material;
    private HealthSCript _healthSCript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _healthSCript = GetComponent<HealthSCript>();
        _healthSCript.OnHit += StartFlash;
        _material = GetComponentInChildren<SpriteRenderer>().material;
    }


    void StartFlash()
    {
        StopAllCoroutines();
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
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

}
