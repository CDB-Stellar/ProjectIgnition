using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallShooter : MonoBehaviour
{
    [SerializeField] private GameObject _fireballPrefab;

    [Header("Fireball Properties")]
    [SerializeField] private float _fireballFireRate;
    [SerializeField] private float _fireballGrowthAmount;
    [SerializeField] private float _fireballGrowthRate; 
    [SerializeField] private AnimationCurve _fireBallGrowth;
    [SerializeField] private float _fireballLaunchSpeed;
        
    private float _fireballCoolDown = 0;
    private float _fireballCharge = 0;
    private bool _chargingFireball = false;
    private FireBall _fireball;

    public float FireballCoolDown => _fireballCoolDown;
    
    public void CreateFireball()
    {
        if(_fireballCoolDown > 0) return;       

        _chargingFireball = true;

        _fireball = Instantiate(
            _fireballPrefab,
            transform.position,
            transform.rotation, transform
        ).GetComponent<FireBall>();

        Debug.Log("_fireball:" + _fireball.name);

        StartCoroutine(ChargeFireBall());
    }
    public void LaunchFireball()
    {
        _chargingFireball = false;

        _fireballCoolDown = _fireballFireRate;
    }

    private IEnumerator ChargeFireBall()
    {
        while (_chargingFireball || _fireball.FullyGrown())
        {
            Debug.Log("_fireball:" + _fireball.name);
            Debug.Log("Evaluate: " + _fireBallGrowth.Evaluate(0));
            _fireballCharge = Mathf.Min(_fireballGrowthAmount + _fireballGrowthAmount, 1);
            
            _fireball.SetSize(_fireBallGrowth.Evaluate(_fireballCharge));
            yield return new WaitForSeconds(_fireballGrowthRate);
        }
    }

}
