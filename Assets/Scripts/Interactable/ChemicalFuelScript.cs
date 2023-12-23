using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class ChemicalFuelScript : MonoBehaviour, IResettable
{
    [SerializeField] PlayerEvents _playerEvents;
    public float combustionTime;
    void Start()
    {
       _playerEvents.onPlayerRespawn += ResetSelf;
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
