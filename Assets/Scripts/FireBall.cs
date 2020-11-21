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
    

    private void Start()
    {        
        rbody = GetComponent<Rigidbody2D>();     
        anim = GetComponent<Animator>();

        // Subsribe Events       
        GameEvents.current.onGrowFireball += Grow; 
        GameEvents.current.onLaunchFireBall += LaunchFireBall;

        GameEvents.current.onPlayerDeath += DeleteFireball;
    }
    private void FixedUpdate()
    {
        if (lifeTime < 0.0f)        
            Destroy(gameObject);           
        if (isLaunched)
            Decay();
    }
    public float GetDamage()
    {
        return lifeTime;
    }
    public void InitalizeFireball(float decayRate, float decayAmount)
    {
        this.decayRate = decayRate;
        this.decayAmount = decayAmount;
    }

    private void LaunchFireBall(Vector3 trajectory)
    {
        if (!isLaunched)
        {
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
        {

            GameEvents.current.FireBallCompleteGrowth();
            return true;
        }
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
            //Debug.Log("Decaying, LifeTime: " + lifeTime + ", FireBallSize(from animator): " + anim.GetFloat("fireballSize"));
        }
        decayTimer += Time.deltaTime;        
    }   
    private void Explode()
    {
        GameEvents.current.ApplyForceToPlayer(transform.position, lifeTime);
        GameEvents.current.onGrowFireball -= Grow;
        GameEvents.current.onLaunchFireBall -= LaunchFireBall;
        
    }
    private void DeleteFireball()
    {
        
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
