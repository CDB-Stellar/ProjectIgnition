using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //to get current scene, for saving
using System.IO; //to work with files, for saving

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
    [SerializeField] private float maxVelocityX;
    [SerializeField] private float maxVelocityY;
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

    public int currentLevel = 1; //for saving, 1 by default
    public string currentCheckpoint; //for saving

    //private Varibles
    private Rigidbody2D rbody;
    private bool hasFireBall = false;
    private bool shootPressed;  
    private bool movePressed;  
    private float fuelUse;
    private int unlockedLevel = 1; //for saving, level 1 unlocked by default

    //tick timers
    private float fireRateTimer;
    private float sizeChangeTimer;



    //--------------------------------------------------------- UNITY RUNTIME -------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        fireRateTimer = fireballCoolDown;
        shootPressed = false;
        movePressed = false;

        GameEvents.current.onFireballExplosion += LaunchPlayer;

        /* Saving stuff > */
        // Get unlocked levels from save data
        string path = Application.persistentDataPath + "/player.savefile"; //where the save file is
        if (File.Exists(path))
        {
            PlayerData data = SaveSystem.LoadPlayer();
            unlockedLevel = data.level;
            Debug.Log("file exists, unlocked level: " + unlockedLevel);
        }
        if (currentLevel > unlockedLevel)
        {
            currentLevel = SceneManager.GetActiveScene().buildIndex;
            SavePlayer(); //save every time the player gets to higher level
        }
        /* < Done Saving stuff */
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
        {
            Vector2 jetDirection = -GetVectorToMousePos() * speed;

            if (Mathf.Abs(rbody.velocity.x) > maxVelocityX)
                jetDirection.x = 0f;

            if (Mathf.Abs(rbody.velocity.y) > maxVelocityY)
                jetDirection.y = 0f;

            rbody.AddForce(jetDirection);
        }



        FireBallManger();       

        if (!hasFireBall && fireRateTimer < fireballCoolDown)
            fireRateTimer += Time.deltaTime;
        
        PointJet(GetVectorToMousePos());
    }
    private Vector2 GetVectorToMousePos()
    {
        Vector2 jetDir = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return jetDir.normalized;
    }

    //--------------------------------------------------------- Fireball Code -------------------------------------------------
    private void FireBallManger()
    {
        if (shootPressed && !hasFireBall)
            CreateFireball();
        else if (!shootPressed && hasFireBall)
            LaunchFireball();
        else if (hasFireBall)
            GrowFireBall();        
    }
    private void CreateFireball()
    {       
        hasFireBall = true;
        fireRateTimer = 0.0f;
        fuelUse = 0.0f;
        Instantiate(fireballPREFAB, fireballSpawn.transform.position, transform.rotation, fireballSpawn).GetComponent<FireBall>();
        GameEvents.current.CreateFireBall(fireballDecayRate, fireballDecayAmount);
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
    private void LaunchFireball()
    {
        GameEvents.current.LaunchFireBall(GetVectorToMousePos() * fireballLaunchSpeed + rbody.velocity);
        hasFireBall = false;
    }

    //--------------------------------------------------------- MOVEMENT CODE -------------------------------------------------
    private void PointJet(Vector3 dir)
    {
        flameJet.eulerAngles = Vector3.forward * (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f);
        fireballSpawn.position = transform.position + dir * spawnRadius;
    }
    public void LaunchPlayer(Vector3 fireballPosition, float fuelTime)
    {
        Vector3 direction = fireballPosition - transform.position;       
        Debug.Log("Explosion from: " + direction + "With Distance of: " + direction.magnitude + "With Strength of: " + (fuel * launchForceFactor) / Mathf.Pow(direction.magnitude, 2f));    
        rbody.AddForce(-direction * (fuelTime * launchForceFactor) / Mathf.Pow(direction.magnitude, 2f) );
    }

    //--------------------------------------------------------- SAVING/LOADING -------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Checkpoint")) //if player enters the checkpoint bounding box
        { 
            currentCheckpoint = other.name; //the checkpoint object
            SavePlayer(); //save every time the player hits a new checkpoint
        } 
    }
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
        Debug.Log("player data has been saved");
    }
    public void LoadSpawn()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        string checkpointName = data.checkpoint;
        Transform bonfire = GameObject.Find(checkpointName).transform;
        Transform spawn = bonfire.Find("PlayerSpawn");
    }
} 
