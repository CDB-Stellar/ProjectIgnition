using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthChunk : MonoBehaviour
{
    public Vector3 dir;
    public float speed;

    private Rigidbody2D rbody;

    // Start is called before the first frame update   
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        Launch();
    }

    public void Initalize(Vector3 dir, float speed)
    {       
        this.dir = dir;
        this.speed = speed;
    }
    private void Launch()
    {
        transform.parent = null;
        rbody.velocity = dir * speed;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ground") || other.CompareTag("PlayerProjectile"))
        {
            Debug.Log(other.name);
            Debug.Log("Dirt Clump Destroyed");
            Destroy(gameObject);
        }
        else
        {
            ;
        }
    }
}

