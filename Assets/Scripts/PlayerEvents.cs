using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvents", menuName = "ScriptableObjects/GameEvents", order = 1)]
public class PlayerEvents : ScriptableObject
{
    public Action onPlayerDeath;
    public Action onPlayerRespawn;
    public Action onPlayerLoadMenu;

    public void PlayerDeath()
    {
        onPlayerDeath?.Invoke();
    }
    public void PlayerRespawn()
    {
        onPlayerRespawn?.Invoke();
    }
    public void PlayerLoadMenu()
    {
        onPlayerLoadMenu?.Invoke();
    }
}
