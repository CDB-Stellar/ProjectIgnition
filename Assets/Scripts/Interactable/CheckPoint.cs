using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class CheckPoint : MonoBehaviour
{
    public float priority;
    [Range(0f,1f)]public float startFuel;

    private ParticleController flame;

    private void Start()
    {
        flame = transform.GetComponentInParent<Transform>().GetChild(1).GetComponent<ParticleController>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collision with player");
            flame.StartEmission();
        }
            
    }
}

