using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //work with scenes

public class LevelTransition : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D trig)
	{
		if (trig.gameObject.CompareTag("Player"))
		{
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //load next scene in queue
        }
	}
}
