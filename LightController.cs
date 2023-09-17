using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    Light _light;
    Material _material;

    [SerializeField, Range(1,10)] float maxIntensity = 5;
    [SerializeField, Range(1,60)] float timeToCooldown = 5;

    bool isOn = false;
    
    static readonly int ColorId = Shader.PropertyToID("_Color");
    static readonly int Intensity = Shader.PropertyToID("_Intensity");

    void Awake()
    {
        _light = GetComponent<Light>();
        _material = GetComponent<MeshRenderer>().material;
        
        _material.SetColor(ColorId, _light.color);
    }

    public void Swap()
    {
        if (isOn)
            TurnOff();
        else
            TurnOn();
    }

    void TurnOn()
    {
        StopAllCoroutines();
        _light.intensity = 1;
        _material.SetFloat(Intensity, maxIntensity);
        isOn = true;
    }

    void TurnOff()
    {
        isOn = false;
        _light.intensity = 0;
        StartCoroutine(CoolDown());
        
        IEnumerator CoolDown()
        {
            float time = 0;
            float delta = (maxIntensity - 1) / timeToCooldown;
            float intensity = maxIntensity;
            
            while (time < timeToCooldown)
            {
                intensity -= delta * Time.deltaTime;
                
                _material.SetFloat(Intensity, intensity);
                
                time += Time.deltaTime;

                yield return null;
            }
            
            _material.SetFloat(Intensity, 1);
        }    
    } 
}
