using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpawnerScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject trap;
    public float spawnRate = 2f;
    float nextSpawn = 0.0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnRate;
            Instantiate (trap, transform.position, Quaternion.identity);
        }
        
    }
}
