using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //to get current scene
using System.IO; //to work with files

public class PlayerSaveLoad : MonoBehaviour
{
    // Public Variables
    public int currentLevel = 1; //level 1 by default

    // Private Variables
    private int unlockedLevel = 1; //level 1 unlocked by default

    // Start is called before the first frame update
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
        GameObject.Find("RespawnCanvas").SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowUI()
    {
        // Show the respawn UI
        GameObject.Find("RespawnCanvas").SetActive(true);
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    // I undid everything else I did until we can figure out the functionality later
    //    if (other.CompareTag("Traps") || other.CompareTag("Enemy"))
    //    {
    //        // Show the respawn UI
    //        //GameObject.Find("RespawnCanvas").SetActive(true);

    //        SceneManager.LoadScene("GameOver");
    //    }
    //}    

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
        Debug.Log("player data has been saved");
    }

    // This function can change, doesn't do anything rn
    //public void LoadSpawn()
    //{
    //    PlayerData data = SaveSystem.LoadPlayer();
    //    string checkpointName = data.checkpoint;
    //    Transform bonfire = GameObject.Find(checkpointName).transform;
    //    Transform spawn = bonfire.Find("PlayerSpawn");
    //}
}