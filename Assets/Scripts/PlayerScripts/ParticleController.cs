using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private ParticleSystem targetParticleSystem;
    private float defaultEmissionRate;
    void Start()
    {
        targetParticleSystem = gameObject.GetComponent<ParticleSystem>();
        defaultEmissionRate = targetParticleSystem.emission.rateOverTime.constant;
    }   
    public void StopEmission()
    {
        ParticleSystem.EmissionModule em = targetParticleSystem.emission;
        em.enabled = false;
    }
    public void StartEmission()
    {
        ParticleSystem.EmissionModule em = targetParticleSystem.emission;
        em.enabled = true;
    }
}
