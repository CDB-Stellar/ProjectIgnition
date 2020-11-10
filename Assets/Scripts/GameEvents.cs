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
    public Action<Vector3> onFireballLaunch;
    public Action onFireBallCompleteGrowth;
    public Action<Vector3, float> onFireballExplosion;

    //Player Events
    public Action onPlayerDeath;
    

    //Fireball Event Methods
    public void GrowFireball(float growthAmount)
    {
        onGrowFireball?.Invoke(growthAmount);
    }
    public void LaunchFireBall(Vector3 dir)
    {     
        onFireballLaunch?.Invoke(dir); // Same as a if(onFireball != null) { onFireballLaunch(); }        
    }
    public void FireBallCompleteGrowth()
    {
        onFireBallCompleteGrowth?.Invoke();
    }
    public void FireballExplosion(Vector3 fireBallPos, float lifeTime)
    {
        onFireballExplosion?.Invoke(fireBallPos, lifeTime);
    }
    
    //PlayerEvents
    public void PlayerDeath()
    {
        onPlayerDeath?.Invoke();
    }


}
