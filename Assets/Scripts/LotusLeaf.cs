using UnityEngine;

public class LotusLeaf : MonoBehaviour, IResettable
{
    [SerializeField] PlayerEvents _playerEvents;
    private ParticleController leftFire, centreFire, rightFire;
    private Animator anim;
    private Collider2D _collider2D;
    public void Start()
    {
        leftFire = transform.GetChild(0).GetComponent<ParticleController>();
        centreFire = transform.GetChild(1).GetComponent<ParticleController>();
        rightFire = transform.GetChild(2).GetComponent<ParticleController>();

        anim = GetComponent<Animator>();

        _collider2D = GetComponent<BoxCollider2D>();

        _playerEvents.onPlayerRespawn += ResetSelf;

    }
    public void DisableSelf()
    {
        leftFire.StopEmission();
        centreFire.StopEmission();
        rightFire.StopEmission();

        _collider2D.enabled = false;
    }
    public void ResetSelf()
    {
        DisableSelf();
        anim.SetBool("burned", false);
        _collider2D.enabled = true;
    }
    private void StartBurn()
    {
        anim.SetBool("burned", true);

        leftFire.StartEmission();
        centreFire.StartEmission();
        rightFire.StartEmission();

    }

    private void OnDisable()
    {
        _playerEvents.onPlayerRespawn -= ResetSelf;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartBurn();
        }
    }

}
