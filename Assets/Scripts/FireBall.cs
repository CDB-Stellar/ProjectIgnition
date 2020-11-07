using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{ 

    private Rigidbody2D rbody;
    private PlayerController playerController;
    private Animator anim;

    private bool isLaunched;
    
    private float lifeTime;
    private float decayTimer;

    private float decayRate;
    private float decayAmount;
    
    // Start is called before the first frame update
    void Start()
    {        
        rbody = GetComponent<Rigidbody2D>();     
        anim = GetComponent<Animator>();

        // Subsribe Events       
        GameEvents.current.onGrowFireball += Grow; 
        GameEvents.current.onFireballLaunch += LaunchFireBall; 
    }
    void FixedUpdate()
    {
        if (lifeTime < 0.0f)
        {
            Debug.Log("Being Destroyed");
            Destroy(gameObject);
        }
            
        if (isLaunched)
            Decay();
    }

    public void InitalizeFireball(float decayRate, float decayAmount)
    {
        this.decayRate = decayRate;
        this.decayAmount = decayAmount;
        Debug.Log("Fireball DecayRate: " + this.decayRate + ", DecayAmount(fuel): " + this.decayAmount);
    }

    private void LaunchFireBall(Vector3 trajectory)
    {
        if (!isLaunched)
        {
            Debug.Log("Parent object is: " + transform.parent.name);
            transform.parent = null;
            rbody.simulated = true;
            isLaunched = true;
            rbody.velocity = trajectory;
        }        
    }
    private void Grow(float growthAmount)
    {
        if (anim != null)
        {
            if (!isLaunched && !FireBallTooBig())
            {
                float currentSize = anim.GetFloat("fireballSize");
                anim.SetFloat("fireballSize", currentSize + growthAmount);
                lifeTime += growthAmount;                  
            }
        }               
    }
    private bool FireBallTooBig()
    {
        if (lifeTime >= 100.0f)
            return true;        
        else
            return false;        
    }
    private void Decay()
    {
        //Debug.Log("Decay Timer: " + decayTimer + ", Decay Rate:" + decayRate);
        if (decayTimer > decayRate)
        {            
            anim.SetFloat("fireballSize", anim.GetFloat("fireballSize") - decayAmount);
            lifeTime -= decayAmount;
            decayTimer = 0.0f;
            Debug.Log("Decaying, LifeTime: " + lifeTime + ", FireBallSize(from animator): " + anim.GetFloat("fireballSize"));
        }
        decayTimer += Time.deltaTime;        
    }   
    private void Explode()
    {
        GameEvents.current.FireballExplosion(transform.position, lifeTime);
        GameEvents.current.onGrowFireball -= Grow; 
        GameEvents.current.onFireballLaunch -= LaunchFireBall; 
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isLaunched)
        {
            Explode();
            Destroy(gameObject);
        }
            
    }    
    
}
