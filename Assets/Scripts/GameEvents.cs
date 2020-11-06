﻿using System;
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

    // Events
    public Action<float, float> onCreateFireball;
    public Action<float> onGrowFireball;
    public Action<Vector3> onFireballLaunch;
    public Action<Vector3, float> onFireballExplosion;

    public void CreateFireBall(float decayRate, float decayAmount)
    {
        onCreateFireball?.Invoke(decayRate, decayAmount);
    }
    public void GrowFireball(float growthAmount)
    {
        onGrowFireball?.Invoke(growthAmount);
    }

    public void LaunchFireBall(Vector3 dir)
    {     
        onFireballLaunch?.Invoke(dir); // Same as a if(onFireball != null) { onFireballLaunch(); }        
    }

    public void FireballExplosion(Vector3 fireBallPos, float lifeTime)
    {
        onFireballExplosion?.Invoke(fireBallPos, lifeTime);
    }
     


}