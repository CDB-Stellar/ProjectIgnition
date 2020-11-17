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

    [Header("Player Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float maxGroundSpeed;
    [SerializeField] private float maxFuel;
    [SerializeField] private float fuel; //In seconds of fireball life
    [SerializeField] private float launchForceFactor;
    [SerializeField] private float layerDetectionRadius;
    [SerializeField] private LayerMask ground;

    [Header("Jet Properties")]
    [SerializeField] private float jetRotationSpeed = 5f;
    [SerializeField] private Vector3 jetRestPosition;

    [Header("Fireball Properties")]
    [SerializeField] private float fireballCoolDown;
    [SerializeField] private float fireballGrowthRate;  //the amount of time in seconds that a growth tick fires
    [SerializeField] private float fireballGrowthAmount;
    [SerializeField] private float fireballLaunchSpeed;
    [SerializeField] private float fireballDecayRate; // length og time in seconds between each decay tick
    [SerializeField] private float fireballDecayAmount; //range

    //private Varibles
    private Rigidbody2D rbody;
    private ParticleController bodyPSController, flameJetPSController;

    private bool hasFireBall = false;
    private bool shootPressed;
    private bool movePressed;
    private bool isFireballMaxSize;
    private bool isOnGround;
    private bool isDead;

    //tick timers
    private float fireRateTimer;
    private float sizeChangeTimer;



    //UNITY RUNTIME-------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        fireRateTimer = fireballCoolDown;
        shootPressed = false;
        movePressed = false;

        bodyPSController = GetComponent<ParticleController>();
        flameJetPSController = flameJet.GetComponent<ParticleController>();

        GameEvents.current.onFireBallCompleteGrowth += FireBallMaxSize;
        GameEvents.current.onApplyForceToPlayer += LaunchPlayer;
    }
    private void Update()
    {
        shootPressed = Input.GetMouseButton(1);
        movePressed = Input.GetMouseButton(0);
    }
    void FixedUpdate()
    {
        if (!isDead)
        {
            isOnGround = IsTouchingLayer(ground);
            if (movePressed)
            {
                Vector2 jetDirection = -GetVectorToMousePos() * speed;

                Debug.Log(isOnGround);
                if (isOnGround && Mathf.Abs(rbody.velocity.x) > maxGroundSpeed)                
                    jetDirection.x = 0f;               
               
                rbody.AddForce(jetDirection);
            }

            FireBallManger();

            if (!hasFireBall && fireRateTimer < fireballCoolDown)
                fireRateTimer += Time.deltaTime;

            PointJet(GetVectorToMousePos());
            ScaleJet();

            PointJet(GetVectorToMousePos());
        }
    }
    // MISC CODE? NOT ENOUGH TO ORGANIZE ---------------------------------------------------------------------------------------------------------------------
    private Vector2 GetVectorToMousePos()
    {
        Vector2 jetDir = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return jetDir.normalized;
    }
    private bool IsTouchingLayer(LayerMask layerMask)
    {
        return Physics2D.OverlapCircle(transform.position, layerDetectionRadius, layerMask);
    }
    private void Refuel(float amount, float maximum)
    {
        fuel = Mathf.Max(fuel, Mathf.Min(maximum * maxFuel, fuel + amount));
    }
    private void Die()
    {
        flameJetPSController.StopEmission();
        GameEvents.current.PlayerDeath();
        isDead = true;
    }

    // FIREBALL CODE -----------------------------------------------------------------------------------------------------------------------------------------
    private void FireBallManger()
    {
        if (shootPressed && !hasFireBall)
            CreateFireball();
        else if (!shootPressed && hasFireBall)
            LaunchFireball();
        else if (hasFireBall && !isFireballMaxSize)
            GrowFireBall();
    }
    private void CreateFireball()
    {
        hasFireBall = true;
        fireRateTimer = 0.0f;
        Instantiate(
            fireballPREFAB,
            transform.position,
            transform.rotation, transform
        ).GetComponent<FireBall>().InitalizeFireball(fireballDecayRate, fireballDecayAmount);
    }
    private void GrowFireBall()
    {
        if (sizeChangeTimer <= fireballGrowthRate)
        {
            fuel -= fireballGrowthAmount;
            GameEvents.current.GrowFireball(fireballGrowthAmount);
            sizeChangeTimer = 0.0f;
        }
    }
    private void FireBallMaxSize()
    {
        isFireballMaxSize = true;
        fuel += fireballGrowthAmount;
    }
    private void LaunchFireball()
    {
        GameEvents.current.LaunchFireBall(GetVectorToMousePos() * fireballLaunchSpeed + rbody.velocity);
        hasFireBall = false;
        isFireballMaxSize = false;
    }

    // MOVEMENT CODE -----------------------------------------------------------------------------------------------------------------------------------------
    private void PointJet(Vector3 dir)
    {
        float angle;

        if (movePressed) // if the mouse is pressed, set the angle to rotate to the mouse position
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        else // otherwise set the angle to the rest position
            angle = Mathf.Atan2(jetRestPosition.y, jetRestPosition.x) * Mathf.Rad2Deg - 90f;

        // Carry out the rotation
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        flameJet.transform.rotation = Quaternion.Slerp(flameJet.transform.rotation, rotation, jetRotationSpeed * Time.deltaTime);
    }
    private void ScaleJet()
    {
        float jetSize = fuel / maxFuel;
        flameJet.localScale = new Vector3(jetSize, jetSize, 1f);
    }
    public void LaunchPlayer(Vector3 entityPosition, float forceMultiplier)
    {
        Vector3 direction = entityPosition - transform.position;
        //Debug.Log("Explosion from: " + direction + "With Distance of: " + direction.magnitude + "With Strength of: " + (fuel * launchForceFactor) / Mathf.Pow(direction.magnitude, 2f));    
        rbody.AddForce(-direction * (forceMultiplier * launchForceFactor) / Mathf.Max(Mathf.Pow(direction.magnitude, 2f), 0.1f));
    }

    // COLLISIONS---------------------------------------------------------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sees if the player collided with fuel
        if (other.CompareTag("Fuel"))
        {
            FuelScript pickup = other.GetComponent<FuelScript>();
            if (pickup == null)
                Debug.LogError("FuelScript Not found");
            else
            {
                Refuel(pickup.fuelAmount, pickup.maxIncrease);
            }
        }

        // Sees if player collided with something that will kill you
        if (other.CompareTag("Traps") || other.CompareTag("Enemy"))
        {
            Die();
        }
    }

    // GIZMOSE------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, layerDetectionRadius);
    }
}
