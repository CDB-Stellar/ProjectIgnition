using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class EarthSpirit : MonoBehaviour, IResettable
{
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float distanceThreshold;
    [SerializeField] private float throwCooldown;
    [SerializeField] private float launchSpeed;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private GameObject earthChunk;

    private Vector3 startPos;
    private Vector3 playerPos;
    private Animator anim;
    private AudioManger audioManger;
    private bool playerFound, canMoveForward, canShoot = true;

    private float throwCooldownTimer;
    void Start()
    {
        anim = GetComponent<Animator>();
        audioManger = GetComponent<AudioManger>();
        GameEvents.current.onPlayerRespawn += ResetSelf;
        GameEvents.current.onPlayerDeath += StopShooting;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        playerFound = SearchForPlayer();

        canMoveForward = CheckForWall();

        if (playerFound && canShoot)
        {
            if (throwCooldownTimer <= 0)
            {
                anim.SetTrigger("throw");                
                throwCooldownTimer = throwCooldown;
                anim.SetBool("isMoving", false);
                audioManger.Play("throw");
                LaunchDirt();
            }            
            else if (Vector3.Distance(transform.position, playerPos) < distanceThreshold && canMoveForward)
            {
                if (playerPos.x < transform.position.x)
                {                
                    transform.Translate(1 * Time.deltaTime * speed, 0, 0);
                    transform.localScale = new Vector2(1, 1);
                }
                else if (playerPos.x > transform.position.x)
                {
                    transform.Translate(-1 * Time.deltaTime * speed, 0, 0);
                    transform.localScale = new Vector2(-1, 1);
                }
                anim.SetBool("isMoving", true);
            }
            else
            {
                anim.SetBool("isMoving", false);
            }
        }
        if (throwCooldownTimer > 0)
        {
            throwCooldownTimer -= Time.deltaTime;
        }       
    }
    private void LaunchDirt()
    {
        Debug.Log(gameObject.name + " is Launching Dirt");
       Instantiate(earthChunk, transform.position, transform.rotation, transform).GetComponent<EarthChunk>().Initalize(playerPos - transform.position, launchSpeed); 
    }
    private bool CheckForWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(transform.position.x + transform.localScale.x, transform.position.y), 0.65f, wallLayer);
        if (hit && hit.transform.CompareTag("turn"))
        {            
            return false;
        }
        else
        {
            return true;
        }
    }
    private void StopShooting()
    {
        canShoot = false;
    }
    private bool SearchForPlayer()
    {       
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (hit)
        {           
            playerPos = hit.transform.position;           
            return true;
        }
        else
        {
            playerPos = transform.position;
            return false;
        }       
    }

    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    public void ResetSelf()
    {
        transform.position = startPos;
        gameObject.SetActive(true);
        canShoot = true;
        playerFound = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            float damage = other.GetComponent<FireBall>().GetSize();
            if (damage > health)
                DisableSelf();

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerPos, 1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanceThreshold);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);      
    }

}
