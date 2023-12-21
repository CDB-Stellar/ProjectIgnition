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
    [SerializeField] private PlayerEvents _playerEvents;

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
        _playerEvents.onPlayerRespawn += HideUI; // Subscribe HideUI Function to PlayerRespawnEvent 
        _playerEvents.onPlayerDeath += ShowUI; // Subscribe Show UI Function to PlayerDeathEvent
        _playerEvents.onPlayerLoadMenu += LoadMainMenu; // Subscribe LoadMainMenu Function to PlayerLoadMenu
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
        _playerEvents.PlayerRespawn();
    }
    public void LoadMenu()
    {
        _playerEvents.PlayerLoadMenu();
    }
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
        Debug.Log("player data has been saved");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
