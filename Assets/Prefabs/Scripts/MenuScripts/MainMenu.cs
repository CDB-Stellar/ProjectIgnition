using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //to be able to load scenes

public class MainMenu : MonoBehaviour
{
    // Play 
    public void PlayLevel1() 
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //load next scene in queue
        SceneManager.LoadScene("Glowing Cave"); //will load specific scene by name
    }

    public void PlayLevel2()
    {
        SceneManager.LoadScene("Rainy Forest");
    }

    public void PlayLevel3()
    {
        SceneManager.LoadScene("Quaking Bog");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT--");
        Application.Quit();
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
