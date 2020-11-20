using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private ParticleSystem targetParticleSystem;
    private float defaultEmissionRate;

    private ParticleSystem.EmissionModule emissionModule;
    void Start()
    {
        targetParticleSystem = gameObject.GetComponent<ParticleSystem>();
        defaultEmissionRate = targetParticleSystem.emission.rateOverTime.constant;
        
        emissionModule = targetParticleSystem.emission;
    }   
    public void StopEmission()
    {

        emissionModule.enabled = false;
    }
    public void StartEmission()
    {

        emissionModule.enabled = true;
    }
    public bool isEmmiting()
    {
        return emissionModule.enabled;
    }
}
