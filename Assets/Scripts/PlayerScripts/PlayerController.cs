using UnityEngine;
using Assets.Scripts;
using System;

public class PlayerController : MonoBehaviour, IResettable
{
    //public Varibles
    [Header("Object References")]
    [SerializeField] private Transform flameJet;
    [SerializeField] private GameObject fireballPREFAB;
    [SerializeField] private CheckPoint currentCheckPoint;

    [Header("Player Properties")]
    [SerializeField] private float normalSpeed;
    [SerializeField] private float chemicalSpeed;

    [SerializeField] private float normalMaxVelocity;
    [SerializeField] private float chemicalMaxVelocity;

    [SerializeField] private float maxFuel;
    [SerializeField] private float fuel; //In seconds of fireball life
    [SerializeField] private float FuelDecayAmount; // the amount of fuel that get depleted

    [SerializeField] private float launchForceFactor;

    [SerializeField] private float layerDetectionRadius;
    [SerializeField] private LayerMask ground;

    [Header("Jet Properties")]
    [SerializeField] private float normalDecayMultiplier;
    [SerializeField] private float chemicalDecayMultiplier;

    [SerializeField] private float jetRotationSpeed = 5f;
    [SerializeField] private Vector3 jetRestPosition;

    [Header("Fireball Properties")]
    [SerializeField] private float fireballCoolDown;
    [SerializeField] private float fireballGrowthRate;  //the amount of time in seconds that a growth tick fires
    [SerializeField] private float fireballGrowthAmount;
    [SerializeField] private float fireballLaunchSpeed;
    [SerializeField] private float fireballDecayRate; // length of time in seconds between each decay tick
    [SerializeField] private float fireballDecayAmount; 

    //private Varibles
    private Rigidbody2D rbody;
    private ParticleController normalBodyFlamePSC, chemicalBodyFlamePSC;
    private ParticleController normalJetFlamePSC, chemicalJetFlamePSC;

    private bool hasFireBall = false;
    private bool shootPressed;
    private bool movePressed;
    private bool isFireballMaxSize;
    private bool isOnGround;
    private bool isDead;
    private bool isInChemicalCombustion;

    //tick timers
    private float fireRateTimer;
    private float sizeChangeTimer;
    private float remainingCombustionTimeTimer;
   
    //UNITY RUNTIME-------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        fireRateTimer = fireballCoolDown;
        shootPressed = false;
        movePressed = false;
        
        //Get the Particle Controllers for the two Types of Bodies
        normalBodyFlamePSC = transform.GetChild(0).GetChild(0).GetComponent<ParticleController>();
        chemicalBodyFlamePSC = transform.GetChild(0).GetChild(1).GetComponent<ParticleController>();

        //Get the Particle Controllers for the two Types of Flamejet
        normalJetFlamePSC = transform.GetChild(1).GetChild(0).GetComponent<ParticleController>();
        chemicalJetFlamePSC = transform.GetChild(1).GetChild(1).GetComponent<ParticleController>();

        GameEvents.current.onFireBallCompleteGrowth += FireBallMaxSize;
        GameEvents.current.onApplyForceToPlayer += LaunchPlayer;

        GameEvents.current.onPlayerDeath += DisableSelf;
        GameEvents.current.onPlayerRespawn += ResetSelf;
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
                switch (isInChemicalCombustion)
                {
                    default:
                    case false:
                        ApplyFlameJet(normalSpeed, normalMaxVelocity, normalDecayMultiplier);
                        break;
                    case true:
                        ApplyFlameJet(chemicalSpeed, chemicalMaxVelocity, chemicalDecayMultiplier);
                        break;
                }
            }

            if (remainingCombustionTimeTimer > 0.0f)
            {
                remainingCombustionTimeTimer -= Time.deltaTime;
                if (remainingCombustionTimeTimer <= 0.0f)
                {
                    SwitchBurnMode();
                }
            }
            

            FireBallManger();

            if (!hasFireBall && fireRateTimer < fireballCoolDown)
                fireRateTimer += Time.deltaTime;

            TransformJet(GetVectorToMousePos());
        }
    }
    private void ApplyFlameJet(float normalSpeed, float normalMaxVelocity, float decayMultiplier)
    {
        Vector2 jetDirection = -GetVectorToMousePos() * normalSpeed;

        if (isOnGround && Mathf.Abs(rbody.velocity.x) > normalMaxVelocity)
            jetDirection.x = 0f;

        rbody.AddForce(jetDirection);
        ReduceFuel(FuelDecayAmount * normalDecayMultiplier);
    }
    // GAMEPLAY CODE -----------------------------------------------------------------------------------------------------------------------------------------
    public void ResetSelf()
    {
        //Reset Player for Respawn
        fuel = maxFuel * currentCheckPoint.startFuel;
        transform.position = currentCheckPoint.transform.position;
        SwitchToNormalBurn();

        isDead = false;
    }
    public void DisableSelf()
    {
        //Disable Player for Death     
        isDead = true;
        chemicalJetFlamePSC.StopEmission();
        normalJetFlamePSC.StopEmission();
    }
    private void CompairCheckPoints(CheckPoint newCheckPoint)
    {
        if (currentCheckPoint == null)
            currentCheckPoint = newCheckPoint;
        else if (newCheckPoint.priority > currentCheckPoint.priority)        
            currentCheckPoint = newCheckPoint;        
    }
    private void ReduceFuel(float amount)
    {
        if (fuel < 0.0f)        
            GameEvents.current.PlayerDeath();
        else
            fuel -= amount;
    }
    private void Refuel(float amount, float maximum)
    {
        fuel = Mathf.Max(fuel, Mathf.Min(maximum * maxFuel, fuel + amount));
    }
    private void SwitchBurnMode()
    {
        if (!isInChemicalCombustion)
        {
            SwitchToChemicalBurn();
        }
        else if (isInChemicalCombustion)
        {
            SwitchToNormalBurn();
        }
    }
    private void SwitchToNormalBurn()
    {
        chemicalBodyFlamePSC.StopEmission();
        chemicalJetFlamePSC.StopEmission();

        normalBodyFlamePSC.StartEmission();
        normalJetFlamePSC.StartEmission();

        isInChemicalCombustion = false;
    }
    private void SwitchToChemicalBurn()
    {
        normalBodyFlamePSC.StopEmission();
        normalJetFlamePSC.StopEmission();

        chemicalBodyFlamePSC.StartEmission();
        chemicalJetFlamePSC.StartEmission();

        isInChemicalCombustion = true;
    }
    private bool IsTouchingLayer(LayerMask layerMask)
    {
        return Physics2D.OverlapCircle(transform.position, layerDetectionRadius, layerMask);
    }
    private Vector2 GetVectorToMousePos()
    {       
        Vector2 jetDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return jetDir.normalized;
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
            ReduceFuel(fireballGrowthAmount);
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
    
    private void TransformJet(Vector3 dir)
    {
        float angle;

        if (movePressed) // if the mouse is pressed, set the angle to rotate to the mouse position
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        else // otherwise set the angle to the rest position
            angle = Mathf.Atan2(jetRestPosition.y, jetRestPosition.x) * Mathf.Rad2Deg - 90f;

        // Carry out the rotation
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        flameJet.transform.rotation = Quaternion.Slerp(flameJet.transform.rotation, rotation, jetRotationSpeed * Time.deltaTime);

        // Scale the Jet to the appropriate size
        float jetSize = fuel / maxFuel;
        foreach (Transform transform in flameJet.GetComponentInChildren<Transform>())
        {
            transform.localScale = new Vector3(jetSize, jetSize, 1f);
        }
        
    }
    private void LaunchPlayer(Vector3 entityPosition, float forceMultiplier)
    {
        Vector3 direction = entityPosition - transform.position;
        //Debug.Log("Explosion from: " + direction + "With Distance of: " + direction.magnitude + "With Strength of: " + (fuel * launchForceFactor) / Mathf.Pow(direction.magnitude, 2f));    
        rbody.AddForce(-direction * (forceMultiplier * launchForceFactor) / Mathf.Max(Mathf.Pow(direction.magnitude, 2f), 0.1f));
    }

    // COLLISIONS---------------------------------------------------------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sees if player collided with something that will kill you
        if (other.CompareTag("Traps") || other.CompareTag("Enemy"))        
            GameEvents.current.PlayerDeath();        

        // Sees if the player collided with fuel
        if (other.CompareTag("Fuel"))
        {
            FuelScript pickup = other.GetComponent<FuelScript>();
            if (pickup == null)
                Debug.LogError("FuelScript Not found");            
            else                                         
                Refuel(pickup.fuelAmount, pickup.maxIncrease);         
        }

        if (other.CompareTag("ChemicalFuel"))
        {
            ChemicalFuelScript pickup = other.GetComponent<ChemicalFuelScript>();
            if (pickup == null)
                Debug.LogError("FuelScript Not found");
            else
            {
                SwitchBurnMode();
                remainingCombustionTimeTimer = pickup.combustionTime;
            }
        }

        // Sees if the player collided with a checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            CheckPoint newCheckPoint = other.GetComponent<CheckPoint>();
            if (newCheckPoint == null)
                Debug.LogError("Collided with Checkpoint, but Script not found");
            else
                CompairCheckPoints(newCheckPoint);
        }
    }

    // GIZMOSE------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, layerDetectionRadius);
    }    
}
