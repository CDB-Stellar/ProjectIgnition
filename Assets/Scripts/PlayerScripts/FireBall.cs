﻿using UnityEngine;

public class FireBall : MonoBehaviour
{
    private Rigidbody2D _rbody;
    private Animator _anim;

    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _steam;
    [SerializeField] private AnimationCurve _growthCurve;

    [SerializeField] private float _decayRate;
    [SerializeField] private float _decayAmount;
    
    private bool _isLaunched;
    private float _size = 0; // 0 - 1 size of fireball
    private float _growth = 0; // 0 - 1 growth of fireball
    private float _decayTimer;

    private void Awake()
    {
        _rbody = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        if (_size <= 0)
            Destroy(gameObject);
        if (_isLaunched)
            Decay();
    }
    public float GetSize()
    {
        return _size;
    }
    public void LaunchFireball(Vector3 trajectory)
    {
        if (!_isLaunched)
        {
            transform.parent = null;
            _rbody.simulated = true;
            _isLaunched = true;
            _rbody.velocity = trajectory;
        }
    }
    public void Grow(float amount)
    {
        _growth = Mathf.Min(_growth + amount, 1f);
        _size = _growthCurve.Evaluate(_growth);
        _anim.SetFloat("fireballSize", _size);
    }
    public bool FullyGrown()
    {
        if (_size >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void Decay()
    {
        if (_decayTimer > _decayRate)
        {
            _anim.SetFloat("fireballSize", _anim.GetFloat("fireballSize") - _decayAmount);
            _size -= _decayAmount;
            _decayTimer = 0.0f;
        }
        _decayTimer += Time.deltaTime;
    }
    private void Explode()
    {
        GameEvents.current.ApplyForceToPlayer(transform.position, _size);
        Instantiate(_explosion, transform.position, _explosion.transform.localRotation);
    }

    public void Extinguish()
    {
        Instantiate(_steam, transform.position, _steam.transform.localRotation);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_isLaunched)
        {
            Explode();
            Destroy(gameObject);
        }

    }

}