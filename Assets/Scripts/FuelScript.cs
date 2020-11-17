using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelScript : MonoBehaviour, IResettable
{
    [Header("Fuel Sprites")]
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private int index;

    [Header("Information The Player Uses")]
    public float fuelAmount;
    public bool isChemical;

    [Range(0f,1f)]public float maxIncrease; //Range from 0 - 1 to make balancing easier
    void Start()
    {
        GameEvents.current.onPlayerRespawn += ResetSelf;

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

    public void OnTriggerEnter2D(Collider2D collision)
    {
        DisableSelf();
    }

}
