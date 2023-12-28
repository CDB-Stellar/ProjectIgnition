using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallShooter : MonoBehaviour
{
    [SerializeField] private GameObject _fireballPrefab;

    [Header("Shooter Properties")]
    [SerializeField] private float _fireballFireRate;
    [SerializeField] private float _fireballLaunchSpeed;

    [Header("Fireball Growth")]
    [SerializeField] private float _fireballGrowthAmount;
    [SerializeField] private float _fireballGrowthRate;
    [SerializeField] private float _fireballCreateCost;
    [SerializeField] private float _fireballChargeCost;
        
    private float _fireballCoolDown = 0;
    private bool _chargingFireball = false;
    private FireBall _fireball;

    public Action<float> OnFireBallCharge;

    public bool CanShoot => !_chargingFireball && (_fireballCoolDown <= 0);
    public float FireballCoolDown => _fireballCoolDown;
    public bool Charging => _chargingFireball;

    public bool HasFireBall => _fireball != null;
    
    public void CreateFireball()
    {
        if(_fireballCoolDown > 0) return;       

        _chargingFireball = true;

        _fireball = Instantiate(
            _fireballPrefab,
            transform.position,
            transform.rotation, transform
        ).GetComponent<FireBall>();

        OnFireBallCharge?.Invoke(_fireballCreateCost);

        StartCoroutine(ChargeFireBall());
    }

    private void Update()
    {
        if(FireballCoolDown > 0)
        {
            _fireballCoolDown = Mathf.Max(_fireballCoolDown - Time.deltaTime, 0f);
        }
    }

    public void Refresh()
    {
        _fireballCoolDown = 0;
        _chargingFireball = false;
    }

    public void LaunchFireball(Vector2 trajectory)
    {
        _chargingFireball = false;
        _fireballCoolDown = _fireballFireRate;

        _fireball.LaunchFireball(trajectory * _fireballLaunchSpeed);
    }

    private IEnumerator ChargeFireBall()
    {
        while ((_chargingFireball || _fireball.FullyGrown()) && HasFireBall)
        {
            _fireball.Grow(_fireballGrowthAmount);
            OnFireBallCharge?.Invoke(_fireballChargeCost);
            yield return new WaitForSeconds(_fireballGrowthRate);
        }
    }

}
