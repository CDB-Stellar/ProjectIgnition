using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEditor.Build;

public class EarthSpirit : MonoBehaviour, IResettable
{
    [SerializeField] private PlayerEvents playerEvents;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private GameObject earthChunk;
    [SerializeField] private GameObject _deathEffect;

    [Header("Behaviour")]
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private bool _moveRight;
    [SerializeField] private float fleeRadius;
    [SerializeField] private float detectionRadius;

    [Header("Projectile Settings")]
    [SerializeField] private float throwCooldown;
    [SerializeField] private float launchSpeed;

    private Vector3 startPos;
    private Vector3 playerPos;
    private Animator anim;
    private AudioManger audioManger;
    private bool _playerInRange;
    private bool _blockedByWall;
    private bool _canShoot = true;


    private float throwCooldownTimer;

    private EarthSpiritStates _state;
    private enum EarthSpiritStates
    {
        Patrol,
        Flee,
        Attack
    }

    private float Direction => transform.localScale.x;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioManger = GetComponent<AudioManger>();
        _state = EarthSpiritStates.Patrol;
        anim.SetBool("isMoving", true);


        playerEvents.onPlayerRespawn += ResetSelf;
        playerEvents.onPlayerDeath += StopShooting;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _playerInRange = PlayerInRange();
        _blockedByWall = WallInWay(Direction);

        switch (_state)
        {
            case EarthSpiritStates.Patrol:
                Patrol();
                break;
            case EarthSpiritStates.Flee:
                Flee();
                break;
            case EarthSpiritStates.Attack:
                Attack();
                break;
            default:
                break;
        }
    }

    private void Patrol()
    {
        // Player comes into View Range
        if (_playerInRange)
        {
            FacePlayer();
            anim.SetBool("isMoving", false);
            _state = EarthSpiritStates.Attack;
            _canShoot = true;
            return;
        }        

        // Spirit comes into contact with a wall
        else if (_blockedByWall)
        {
            SwitchDirection();
        }

        transform.Translate(Direction * Time.deltaTime * speed, 0, 0);
    }

    private void Flee()
    {
        // Player Leaves Flee Range
        if (DistanceToPlayer() > fleeRadius)
        {
            FacePlayer();
            _state = EarthSpiritStates.Attack;
            anim.SetBool("isMoving", false);
            _canShoot = true;
            return;
        }
        // Spirit Gets Cornered       
        else if (_blockedByWall)
        {
            FacePlayer();
            _state = EarthSpiritStates.Attack;
            anim.SetBool("isMoving", false);
            _canShoot = true;
            return;
        }


        // Move Player Left
        if (playerPos.x < transform.position.x)
        {
            transform.Translate(1 * Time.deltaTime * speed, 0, 0);
            transform.localScale = new Vector2(1, 1);
        }
        // Move Player Right
        else if (playerPos.x > transform.position.x)
        {
            transform.Translate(-1 * Time.deltaTime * speed, 0, 0);
            transform.localScale = new Vector2(-1, 1);
        }
    }

    private void Attack()
    {
        // Player Leaves View Range
        if (!_playerInRange)
        {
            _state = EarthSpiritStates.Patrol;
            anim.SetBool("isMoving", true);
            _canShoot = false;
            return;

        }
        // Player Comes into Flee Range and is not cornered
        else if (DistanceToPlayer() < fleeRadius && !WallInWay(-Direction))
        {
            _state = EarthSpiritStates.Flee;
            anim.SetBool("isMoving", true);
            _canShoot = false;
            return;
        }
        
        // Attack
        if (_canShoot && throwCooldownTimer < 0f)
        {
            LaunchDirt();
        }
        else
        {
            throwCooldownTimer -= Time.deltaTime;
        }

    }

    private void SwitchDirection()
    {
        _moveRight = !_moveRight;
        transform.localScale = new Vector2(transform.localScale.x * -1, 1);
    }

    private void FacePlayer()
    {
        // player left
        if (playerPos.x < transform.position.x)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        // player Right
        else if (playerPos.x > transform.position.x)
        {
            transform.localScale = new Vector2(1, 1);
        }
    }

    private float DistanceToPlayer()
    {
        return Vector2.Distance(transform.position, playerPos);
    }

    private void LaunchDirt()
    {
        Debug.Log("THROWING");

        anim.SetTrigger("throw");
        anim.SetBool("isMoving", false);
        audioManger.Play("throw");

        throwCooldownTimer = throwCooldown;
        Instantiate(earthChunk, transform.position, transform.rotation, transform)
            .GetComponent<EarthChunk>()
            .Initalize((playerPos - transform.position).normalized, launchSpeed);
    }
    private bool WallInWay(float direction)
    {
        Vector2 dir = new Vector2(direction, 0f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1.5f, wallLayer);
        if (hit && hit.transform.CompareTag("turn"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void StopShooting()
    {
        _canShoot = false;
    }
    private bool PlayerInRange()
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
        Instantiate(_deathEffect, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    public void ResetSelf()
    {
        transform.position = startPos;
        gameObject.SetActive(true);
        _canShoot = false;
        _playerInRange = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        FireBall fireBall;
        if (other.TryGetComponent(out fireBall))
        {
            fireBall.Explode();
            float damage = fireBall.GetSize();
            if (damage > health)
            {
                DisableSelf();
            }
        }       
    }

    private void OnDrawGizmos()
    {
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawWireSphere(playerPos, 1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fleeRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

}
