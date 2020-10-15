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
    [SerializeField] private GameObject fireball;
    [SerializeField] private Transform fireballSpawn;

    [Header("Player Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float fireballCoolDown;

    //private Varibles
    private Rigidbody2D rbody;
    private bool hasFireBall;
    private float timer;
    public event EventHandler<OnMouse1ReleasedArgs> OnMouse1Released;
    public class OnMouse1ReleasedArgs : EventArgs
    {
        public Vector3 dir;
    }

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        timer = fireballCoolDown;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetAxis("Mouse 0") > 0.05f)        
            rbody.AddForce(-GetJetDirection() * speed);        

        if (Input.GetAxis("Mouse 1") > 0.0f && !hasFireBall && timer > fireballCoolDown)                   
            CreateFireball();       
        else if (Input.GetAxis("Mouse 1") <= 0.0f && hasFireBall)        
            LaunchFireball();

        PointJet(GetJetDirection());
        timer += Time.deltaTime;
    }    
    
    //Returns a direction vector pointed at the mouse normalized
    private Vector2 GetJetDirection()
    {
        Vector2 jetDir = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return jetDir.normalized;
    }
    private void PointJet(Vector3 dir)
    {
        flameJet.eulerAngles = Vector3.forward * (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        fireballSpawn.position = transform.position + dir;
    }
    private void CreateFireball()
    {
        GameObject projectile = (GameObject)Instantiate(fireball, fireballSpawn.transform.position, transform.rotation, fireballSpawn);
        hasFireBall = true;
        timer = 0.0f;
    }
    private void LaunchFireball()
    {
        OnMouse1Released?.Invoke(this, new OnMouse1ReleasedArgs { dir = GetJetDirection() });
        hasFireBall = false;
    }
    
} 
