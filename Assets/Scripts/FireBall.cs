using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{ 

    private Rigidbody2D rbody;
    private PlayerController playerController;
    private Animator anim;

    private bool hasLaunched;
    
    private float lifeTime;
    private float decayTimer;

    private float decayRate;
    private float decayAmount;
    
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();
        rbody = GetComponent<Rigidbody2D>();     
        anim = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        if (lifeTime < 0.0f)
        {
            Debug.Log("Being Destroyed");
            Destroy(gameObject);
        }
            
        if (hasLaunched)
            Decay();
    }

    public void LaunchFireBall(Vector3 trajectory, float decayRate, float decayAmount)
    {    
        transform.parent = null;
        rbody.simulated = true;
        hasLaunched = true;
        rbody.velocity = trajectory;

        this.decayAmount = decayAmount;
        this.decayRate = decayRate;
    }   
    public void Grow(float growthAmount)
    {
        if (anim != null)
        {
            float currentSize = anim.GetFloat("fireballSize");
            anim.SetFloat("fireballSize", currentSize + growthAmount);
            lifeTime += growthAmount;
            //Debug.Log(transform.localScale.magnitude);
        }               
    }
    public bool FireBallTooBig()
    {
        if (lifeTime >= 100.0f)
            return true;
        
        else
            return false;        
    }
    private void Decay()
    {
        if (decayTimer < decayRate)
        {
            anim.SetFloat("fireballSize", anim.GetFloat("fireballSize") - decayAmount);
            lifeTime -= decayAmount;
            decayTimer = 0.0f;
        }
        decayTimer += Time.deltaTime;        
    }   
    private void explode()
    {
        playerController.Launch(transform.position, lifeTime);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (hasLaunched)
        {
            explode();
            Destroy(gameObject);
        }
            
    }    
    
}
