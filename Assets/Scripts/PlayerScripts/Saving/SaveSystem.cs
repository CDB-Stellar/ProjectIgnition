using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; //to work with files
using System.Runtime.Serialization.Formatters.Binary; //to save in binary file

public static class SaveSystem
{
    // This does the saving a loading of player data - the current level.

    public static void SavePlayer(PlayerLoader player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.savefile"; //where the save file is

        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerData saveData = new PlayerData(player);

        // Insert data into the player.savefile
        formatter.Serialize(stream, saveData);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.savefile"; //where the save file is
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open); //open save file

            // Get the info from the file
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in" + path);
            return null;
        }
    }
}
