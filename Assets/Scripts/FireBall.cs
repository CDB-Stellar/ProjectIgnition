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
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(hasLaunched)
            Destroy(gameObject);
    }
    public void Launch(Vector3 dir)
    {
        transform.parent = null;
        rbody.gravityScale = gForce;
        rbody.AddForce(dir * launchSpeed);
    }
}
