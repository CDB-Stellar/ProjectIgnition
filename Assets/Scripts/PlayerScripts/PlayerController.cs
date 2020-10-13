using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //public Varibles
    [Header("Object References")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform flameJet;
    [SerializeField] private Object fireball;
    [SerializeField] private Transform fireballSpawn;

    [Header("Player Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float maxVelocity;

    //private Varibles
    private Rigidbody2D rbody;
    private bool hasFireBall;
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetAxis("Mouse 0") > 0.0f)
        {
            rbody.AddForce(-GetJetDirection() * speed);
        }
        PointJet(GetJetDirection());

        if (Input.GetAxis("Mouse 1") > 0.0f && !hasFireBall)
        {
            CreateFireball(GetJetDirection());         
        }
    }
    //Returns a direction vector pointed at the mouse normalized
    private Vector2 GetJetDirection()
    {
        Vector2 jetDir =  cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //Debug.DrawRay(transform.position, jetDir.normalized);
        return jetDir.normalized;          
    }
    private void PointJet(Vector3 dir)
    {
        flameJet.eulerAngles = Vector3.forward * (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        fireballSpawn.position = transform.position + dir;
    }  
    private void CreateFireball(Vector2 dir)
    {
        GameObject projectile = (GameObject)Instantiate(fireball, fireballSpawn.transform.position, transform.rotation, fireballSpawn);
        hasFireBall = true;
    }
   
}
