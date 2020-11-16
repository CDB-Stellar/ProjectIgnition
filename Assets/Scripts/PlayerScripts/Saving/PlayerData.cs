using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable] //ability to save it in a file
public class PlayerData //no monobehaviour
{
    // This class just gets player data to save the level progress. SaveSystem does the actual saving.

    public int level = 1; //player's current level, 1 by default  

    public PlayerData(PlayerLoader player)
    {
        level = player.currentLevel; //getting the current level from PlayerController        
    }
}
