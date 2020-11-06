using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //to get current scene, for saving
using System.IO; //to work with files, for saving

public class PlayerSavingLoading : MonoBehaviour
{
    public int currentLevel = 1; //for saving, 1 by default
    public string currentCheckpoint; //for saving

    private int unlockedLevel = 1; //for saving, level 1 unlocked by default

    // Start is called before the first frame update
    void Start()
    {
        /* Saving stuff > */
        // Get unlocked levels from save data
        string path = Application.persistentDataPath + "/player.savefile"; //where the save file is
        if (File.Exists(path))
        {
            PlayerData data = SaveSystem.LoadPlayer();
            unlockedLevel = data.level;
            Debug.Log("file exists, unlocked level: " + unlockedLevel);
        }
        if (currentLevel > unlockedLevel)
        {
            currentLevel = SceneManager.GetActiveScene().buildIndex;
            SavePlayer(); //save every time the player gets to higher level
        }
        /* < Done Saving stuff */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //--------------------------------------------------------- SAVING/LOADING -------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Checkpoint")) //if player enters the checkpoint bounding box
        {
            currentCheckpoint = other.name; //the checkpoint object
            SavePlayer(); //save every time the player hits a new checkpoint
        }
    }
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
        Debug.Log("player data has been saved");
    }
    public void LoadSpawn()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        string checkpointName = data.checkpoint;
        Transform bonfire = GameObject.Find(checkpointName).transform;
        Transform spawn = bonfire.Find("PlayerSpawn");
    }
}
