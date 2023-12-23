using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthChunk : MonoBehaviour
{
    [SerializeField] private PlayerEvents _playerEvents;
    [SerializeField] private LayerMask _collideLayer;
    [SerializeField] private GameObject _combustEffect;
    [SerializeField] private GameObject _collideEffect;

    public Vector3 dir;
    public float speed;

    private Rigidbody2D rbody;

    // Start is called before the first frame update   
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        Launch();
    }

    private void OnEnable()
    {
        _playerEvents.onPlayerDeath += DestroySelf;
    }

    private void OnDisable()
    {
        _playerEvents.onPlayerDeath -= DestroySelf;
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

    private void Collide()
    {
        Instantiate(_collideEffect, transform.position, Quaternion.identity);
        DestroySelf();
    }

    private void Combust()
    {
        Instantiate(_combustEffect, transform.position, Quaternion.identity);
        DestroySelf();
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FireBall fireBall;
        if (other.TryGetComponent(out fireBall))
        {
            fireBall.Explode();
            Combust();
        }

        else if (other.gameObject.GetComponent<EarthSpirit>() == null)
        {
            Debug.Log("Being Destroyed by " + other.gameObject.name);
            Collide();
        }
    }
}

