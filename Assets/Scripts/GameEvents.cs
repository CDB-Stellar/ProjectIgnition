using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    // FireBall Events
    public Action<float> onGrowFireball;
    public Action<Vector3> onLaunchFireBall;
    public Action onFireBallCompleteGrowth;
    public Action<Vector3, float> onApplyForceToPlayer;

    //Player Events
    public Action onPlayerDeath;
    public Action onPlayerRespawn;
    

    //Fireball Event Methods ---------------------------------------------------------------------------------------------------------------------------------
    public void GrowFireball(float growthAmount)
    {
        onGrowFireball?.Invoke(growthAmount);
    }
    public void LaunchFireBall(Vector3 dir)
    {
        onLaunchFireBall?.Invoke(dir); // Same as a if(onFireball != null) { onApplyForceOnPlayer(); }        
    }
    public void FireBallCompleteGrowth()
    {
        onFireBallCompleteGrowth?.Invoke();
    }
    public void ApplyForceToPlayer(Vector3 fireBallPos, float lifeTime)
    {
        onApplyForceToPlayer?.Invoke(fireBallPos, lifeTime);
    }
    
    //PlayerEvents -------------------------------------------------------------------------------------------------------------------------------------------
    public void PlayerDeath()
    {
        onPlayerDeath?.Invoke();
    }
    public void PlayerRespawn()
    {
        onPlayerRespawn?.Invoke();
    }


}
