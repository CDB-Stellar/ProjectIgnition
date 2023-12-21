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
    public Action<Vector3, float> onApplyForceToPlayer;
       
    // Fireball Event Methods ---------------------------------------------------------------------------------------------------------------------------------
    public void ApplyForceToPlayer(Vector3 fireBallPos, float strength)
    {
        onApplyForceToPlayer?.Invoke(fireBallPos, strength);
    }
}
