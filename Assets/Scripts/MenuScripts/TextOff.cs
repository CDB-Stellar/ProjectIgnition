using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextOff : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject.Find("EndTextCanvas").SetActive(false); //disable itself
        }
    }
}
