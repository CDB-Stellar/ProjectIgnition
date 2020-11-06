using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable] //ability to save it in a file
public class PlayerData //no monobehaviour
{
    // This class just gets player data to save the level progress. SaveSystem does the actual saving.

    public int level = 1; //player's current level, 1 by default
    public string checkpoint = "1_BonfireCheckpoint"; //player's current checkpoint name
    //public float[] spawn; //checkpoint spawn

    public PlayerData(PlayerSaveLoad player)
    {
        level = player.currentLevel; //getting the current level from PlayerController
        checkpoint = player.currentCheckpoint; //current checkpoint from PlayerController

        //spawn = new float[3];
        //spawn[0] = player.transform.position.x;
        //spawn[1] = player.transform.position.y;
        //spawn[2] = player.transform.position.z;
    }
}
