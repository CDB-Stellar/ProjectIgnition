using UnityEngine;

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

    private void OnDisable()
    {
       _playerEvents.onPlayerRespawn -= ResetSelf;

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        DisableSelf();
    }
}
