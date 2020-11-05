using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; //to work with files

public class AccessLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get unlocked levels from save data
        int unlockedLevel = 1; //level 1 unlocked by default
        string path = Application.persistentDataPath + "/player.savefile"; //where the save file is
        if (File.Exists(path))
        {
            PlayerData data = SaveSystem.LoadPlayer();
            unlockedLevel = data.level;
            Debug.Log("file exists, unlocked level: " + unlockedLevel);
        }
        else
            Debug.Log("file doesn't exist, unlocked level: " + unlockedLevel);

        // Unlock appropriate levels
        if (unlockedLevel == 1)
        {
            GameObject.Find("Level2Button").SetActive(false);
            GameObject.Find("Level3Button").SetActive(false);
        }
        else if (unlockedLevel == 2)
        {
            GameObject.Find("Level3Button").SetActive(false);
        }
    }
}
