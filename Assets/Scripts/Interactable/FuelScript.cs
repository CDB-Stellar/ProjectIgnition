﻿using System.Collections.Generic;
using UnityEngine;

public class FuelScript : MonoBehaviour, IResettable
{
    [SerializeField] PlayerEvents _playerEvents;

    [Header("Fuel Sprites")]
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private int index;
    [SerializeField] private GameObject itemFeedback; //drag in feedbackAnimation prefab

    public float fuelAmount;

    [Range(0f,1f)]public float maxIncrease; //Range from 0 - 1 to make balancing easier
    void Start()
    {
        _playerEvents.onPlayerRespawn += ResetSelf;

        GetComponent<SpriteRenderer>().sprite = sprites[index];

    }

    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    public void ResetSelf()
    {
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _playerEvents.onPlayerRespawn -= ResetSelf;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) //if the player touches the fuel's collider
        {
            GameObject.Instantiate(itemFeedback, this.transform.position, this.transform.rotation); //make the item feedback play
            DisableSelf();
        }
    }
}
