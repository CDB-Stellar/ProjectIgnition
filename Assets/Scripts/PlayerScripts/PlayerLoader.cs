using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;

public class PlayerLoader : MonoBehaviour
{
    // Public Variables
    public int currentLevel = 1; //level 1 by default
    public GameObject respawnUI;

    // Private Variables
    private int unlockedLevel = 1; //level 1 unlocked by default
    void Start()
    {
        // Unlock certain levels from the main menu
        // Get unlocked levels from save data
        string path = Application.persistentDataPath + "/player.savefile"; //where the save file is
        if (File.Exists(path))
        {
            PlayerData data = SaveSystem.LoadPlayer(); //load save data
            unlockedLevel = data.level;
            Debug.Log("file exists, unlocked level: " + unlockedLevel);
        }

        currentLevel = SceneManager.GetActiveScene().buildIndex;
        if (currentLevel > unlockedLevel) //if the current level is a higher build index than unlocked level
            SavePlayer(); //save every time the player gets to higher level

        // Hide the respawn UI
        HideUI();

        // Game Event
        GameEvents.current.onPlayerRespawn += HideUI; // Subscribe HideUI Function to PlayerRespawnEvent 
        GameEvents.current.onPlayerDeath += ShowUI; // Subscribe Show UI Function to PlayerDeathEvent
    }
    private void HideUI()
    {
        respawnUI.SetActive(false);
    }
    private void ShowUI()
    {
        respawnUI.SetActive(true); // Show the respawn UI
    }
    public void Respawn()
    {
        GameEvents.current.PlayerRespawn();
    }
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
        Debug.Log("player data has been saved");
    }
}
