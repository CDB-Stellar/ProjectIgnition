using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    //public varibles
    [Header("FireBall Properties")]
    [SerializeField] private float gForce;
    [SerializeField] private float launchSpeed;

    private Rigidbody2D rbody;
    private bool hasLaunched;
    private PlayerController playerController;
    private float lifeInSeconds = 5f;
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        playerController = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();
        playerController.OnMouse1Released += LaunchFireBall;
        Debug.Log(playerController);
    }
    private void LaunchFireBall(object Sender, PlayerController.OnMouse1ReleasedArgs e)
    {       
        playerController.OnMouse1Released -= LaunchFireBall;
        transform.parent = null;
        rbody.simulated = true;
        hasLaunched = true;
        rbody.AddForce(e.dir * launchSpeed);        
    }

    // Update is called once per frame
    void Update()
    {
        if (hasLaunched)        
            lifeInSeconds -= Time.deltaTime;        
        if (lifeInSeconds < 0.0f)        
            Destroy(gameObject);
        
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(hasLaunched)
            Destroy(gameObject);
        
    }    
    
}
