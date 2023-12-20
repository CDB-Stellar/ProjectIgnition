using Assets.Scripts;
using UnityEngine;

public class PlayerController : MonoBehaviour, IResettable
{
    //public Varibles
    [Header("Object References")]
    [SerializeField] private Transform _flameJet;
    [SerializeField] private CheckPoint _currentCheckPoint;
    [SerializeField] private FireBallShooter _fireBallShooter;

    [Header("Player Properties")]

    [Header("Normal Movement")]
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxVelocityAirX;
    [SerializeField] private float maxVelocityAirY;

    [Header("Chemical Combustion Movement")]
    [SerializeField] private float chemicalAcceleration;
    [SerializeField] private float chemicalMaxSpeed;
    [SerializeField] private float chemicalMaxVelocityAirX;
    [SerializeField] private float chemicalMaxVelocityAirY;

    [Header("Fuel")]

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

    //private Varibles
    private Rigidbody2D rbody;
    private ParticleController normalBodyFlamePSC, chemicalBodyFlamePSC;
    private ParticleController normalJetFlamePSC, chemicalJetFlamePSC;
    private AudioManger audioManger;

    private bool _shootPressed;
    private bool _movePressed;
    private bool _isOnGround;
    private bool _isDead;
    private bool _isInChemicalCombustion;

    private float _remainingCombustionTime;

    

    //UNITY RUNTIME-------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        _shootPressed = false;
        _movePressed = false;

        //Get the Particle Controllers for the two Types of Bodies
        normalBodyFlamePSC = transform.GetChild(0).GetChild(0).GetComponent<ParticleController>();
        chemicalBodyFlamePSC = transform.GetChild(0).GetChild(1).GetComponent<ParticleController>();

        //Get the Particle Controllers for the two Types of Flamejet
        normalJetFlamePSC = transform.GetChild(1).GetChild(0).GetComponent<ParticleController>();
        chemicalJetFlamePSC = transform.GetChild(1).GetChild(1).GetComponent<ParticleController>();

        GameEvents.current.onApplyForceToPlayer += LaunchPlayer;

        GameEvents.current.onPlayerDeath += DisableSelf;
        GameEvents.current.onPlayerRespawn += ResetSelf;

        audioManger = GetComponent<AudioManger>();
    }
    private void Update()
    {
        _shootPressed = Input.GetMouseButton(1);
        _movePressed = Input.GetMouseButton(0);
    }
    void FixedUpdate()
    {
        if (!_isDead)
        {
            _isOnGround = IsTouchingLayer(ground);

            if (_movePressed)
            {
                switch (_isInChemicalCombustion)
                {
                    default:
                    case false:
                        ApplyFlameJet(
                            acceleration,
                            maxSpeed,
                            maxVelocityAirX,
                            maxVelocityAirY,
                            normalDecayMultiplier);
                        break;
                    case true:
                        ApplyFlameJet(
                            chemicalAcceleration,
                            chemicalMaxSpeed,
                            chemicalMaxVelocityAirX,
                            chemicalMaxVelocityAirY,
                            chemicalDecayMultiplier);
                        break;

                }
            }

            //Debug.Log("mag: " + rbody.velocity.magnitude);


            if (_remainingCombustionTime > 0.0f)
            {
                _remainingCombustionTime -= Time.deltaTime;
                if (_remainingCombustionTime <= 0.0f)
                {
                    SwitchToNormalBurn();
                }
            }


            FireBallManger();

            TransformJet(GetVectorFromMousePos());
        }
    }
    public bool IsIncapacitated()
    {
        return _isDead;
    }
    // GAMEPLAY CODE -----------------------------------------------------------------------------------------------------------------------------------------
    private void FireBallManger()
    {
        if (_shootPressed && _fireBallShooter.CanShoot)
        {
            _fireBallShooter.CreateFireball();
        }
        else if (!_shootPressed && _fireBallShooter.Charging)
        {
            _fireBallShooter.LaunchFireball(GetVectorFromMousePos());
        }
    }
    public void ResetSelf()
    {
        //Reset Player for Respawn
        fuel = maxFuel * _currentCheckPoint.startFuel;
        transform.position = _currentCheckPoint.transform.position;
        SwitchToNormalBurn();

        _isDead = false;
    }
    public void DisableSelf()
    {
        //Disable Player for Death     
        _isDead = true;
        chemicalJetFlamePSC.StopEmission();
        normalJetFlamePSC.StopEmission();

        normalBodyFlamePSC.StopEmission();
        chemicalBodyFlamePSC.StopEmission();

        audioManger.Play("fizzle");
    }
    private void CompairCheckPoints(CheckPoint newCheckPoint)
    {
        if (_currentCheckPoint == null)
            _currentCheckPoint = newCheckPoint;
        else if (newCheckPoint.priority > _currentCheckPoint.priority)
            _currentCheckPoint = newCheckPoint;
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
        if (!_isInChemicalCombustion)
        {
            SwitchToChemicalBurn();
        }
        else if (_isInChemicalCombustion)
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

        _isInChemicalCombustion = false;
    }
    private void SwitchToChemicalBurn()
    {
        normalBodyFlamePSC.StopEmission();
        normalJetFlamePSC.StopEmission();

        chemicalBodyFlamePSC.StartEmission();
        chemicalJetFlamePSC.StartEmission();

        _isInChemicalCombustion = true;
    }
    private bool IsTouchingLayer(LayerMask layerMask)
    {
        return Physics2D.OverlapCircle(transform.position, layerDetectionRadius, layerMask);
    }
    private Vector2 GetVectorFromMousePos()
    {
        Vector2 jetDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return jetDir.normalized;
    }

    // MOVEMENT CODE -----------------------------------------------------------------------------------------------------------------------------------------

    private void TransformJet(Vector3 dir)
    {
        float angle;

        if (_movePressed) // if the mouse is pressed, set the angle to rotate to the mouse position
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        else // otherwise set the angle to the rest position
            angle = Mathf.Atan2(jetRestPosition.y, jetRestPosition.x) * Mathf.Rad2Deg - 90f;

        // Carry out the rotation
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _flameJet.transform.rotation = Quaternion.Slerp(_flameJet.transform.rotation, rotation, jetRotationSpeed * Time.deltaTime);

        // Scale the Jet to the appropriate size
        float jetSize = fuel / maxFuel;
        foreach (Transform transform in _flameJet.GetComponentInChildren<Transform>())
        {
            transform.localScale = new Vector3(jetSize, jetSize, 1f);
        }

    }
    private void ApplyFlameJet(float acceleration, float maxSpeed, float maxVelAirX, float maxVelAirY, float decayMultiplier)
    {
        Vector2 dir = -GetVectorFromMousePos() * acceleration;
        Vector2 vel = rbody.velocity;

        // In Air
        if (!_isOnGround)
        {
            // Prevent player from using jet to climb
            if (vel.y > maxVelAirY)
            {                
                dir.y = Mathf.Min(dir.y, -Physics2D.gravity.y * 0.75f);
            }

            // Allow Player to use jet to slow fall rapidly
            if (vel.y < maxVelAirY)
            {
                float velComponentY = Vector2.Dot(Vector2.down, vel.normalized);
                dir.y += -Physics2D.gravity.y * velComponentY;
            }
            
            rbody.AddForce(dir);            
            rbody.velocity = Vector2.ClampMagnitude(rbody.velocity, maxVelAirX);

        }
        else // On ground
        {           
            rbody.AddForce(dir);
            rbody.velocity = Vector2.ClampMagnitude(rbody.velocity, maxSpeed);
        }

        ReduceFuel(FuelDecayAmount * decayMultiplier);
    }
    private void LaunchPlayer(Vector3 entityPosition, float forceMultiplier)
    {
        Vector3 direction = entityPosition - transform.position;
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
                fuel = maxFuel;
                _remainingCombustionTime = pickup.combustionTime;
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
