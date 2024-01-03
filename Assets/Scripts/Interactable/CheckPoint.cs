using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public float priority;
    [Range(0f,1f)]public float startFuel;

    private ParticleController flame;
    private AudioManger audioManger;
    private bool isLit = false;

    private void Start()
    {
        flame = transform.GetComponentInParent<Transform>().GetChild(1).GetComponent<ParticleController>();
        audioManger = GetComponent<AudioManger>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isLit)
            {
                audioManger.Play("Ignite");
                isLit = true;
            }
            flame.StartEmission();
        }
            
    }
}

