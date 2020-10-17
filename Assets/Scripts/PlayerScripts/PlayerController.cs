using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //public Varibles
    [Header("Object References")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform flameJet;
    [SerializeField] private GameObject fireballPREFAB;
    [SerializeField] private Transform fireballSpawn;

    [Header("Player Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float fuel; //In seconds of fireball life
    [SerializeField] private float launchForceFactor;

    [Header("Fireball Properties")]
    [SerializeField] private float fireballCoolDown;
    [SerializeField] private float fireballGrowthRate;  //the amount of time in seconds that a growth tick fires
    [SerializeField] private float fireballGrowthAmount; 
    [SerializeField] private float fireballLaunchSpeed;
    [SerializeField] private float fireballDecayRate; // length og time in seconds between each decay tick
    [SerializeField] private float fireballDecayAmount; //range
    [SerializeField] private float spawnRadius; //range

    //private Varibles
    private Rigidbody2D rbody;
    private FireBall dockedFireball;
    private bool hasFireBall = false;
    private bool shootPressed;  
    private bool movePressed;  
    private float fuelUse;

    //tick timers
    private float fireRateTimer;
    private float sizeChangeTimer;

    
    
    //---------------------------------------------------------UNITY RUNTIME-----------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        fireRateTimer = fireballCoolDown;
        shootPressed = false;
        movePressed = false;
    }
    private void Update()
    {
        shootPressed = Input.GetMouseButton(1);      
        movePressed = Input.GetMouseButton(0);      
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movePressed)        
            rbody.AddForce(-GetJetDirection() * speed);     

        FireBallManger();       

        if (!hasFireBall && fireRateTimer < fireballCoolDown)
            fireRateTimer += Time.deltaTime;
        
        PointJet(GetJetDirection());        
    }
    
    //---------------------------------------------------------Fireball Code-----------------------------------------------------------------------------
    private void FireBallManger()
    {
        if (shootPressed && !hasFireBall)
            dockedFireball = CreateFireball();
        else if (!shootPressed && hasFireBall)
            LaunchFireball();
        else if (hasFireBall && !dockedFireball.FireBallTooBig())
            GrowFireBall(dockedFireball);

        if (hasFireBall)
        {
            Debug.DrawRay(transform.position, transform.position - dockedFireball.transform.position, Color.green);
        }
    }
    private FireBall CreateFireball()
    {       
        hasFireBall = true;
        fireRateTimer = 0.0f;
        fuelUse = 0.0f;
        return Instantiate(fireballPREFAB, fireballSpawn.transform.position, transform.rotation, fireballSpawn).GetComponent<FireBall>();
    }
    private void GrowFireBall(FireBall ball)
    {
        if (sizeChangeTimer <= fireballGrowthRate)
        {
            fuel -= fireballGrowthAmount;
            dockedFireball.Grow(fireballGrowthAmount);
            sizeChangeTimer = 0.0f;
        }       
    }
    private void LaunchFireball()
    {
        Debug.Log("Shooting fireball");
        dockedFireball.LaunchFireBall(ShootDirection(), fireballDecayRate, fireballDecayAmount);
        hasFireBall = false;
        dockedFireball = null;
    }
    private Vector3 ShootDirection()
    {
        return GetJetDirection() * fireballLaunchSpeed + rbody.velocity;
    }


    //----------------------------------------------------------------------- MOVEMENT CODE ------------------------------------------------------------------------
    private Vector2 GetJetDirection()
    {
        Vector2 jetDir = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return jetDir.normalized;
    }
    private void PointJet(Vector3 dir)
    {
        flameJet.eulerAngles = Vector3.forward * (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        fireballSpawn.position = transform.position + dir * spawnRadius;
    }
    public void Launch(Vector3 fireballPosition, float fuelTime)
    {
        Vector3 direction = fireballPosition - transform.position;       
        Debug.Log("Explosion from: " + direction + "With Distance of: " + direction.magnitude + "With Strength of: " + (fuel * launchForceFactor) / Mathf.Pow(direction.magnitude, 2f));    
        rbody.AddForce(-direction * (fuelTime * launchForceFactor) / Mathf.Pow(direction.magnitude, 2f) );
    }    
} 
