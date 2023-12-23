using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class LotusLeaf : MonoBehaviour, IResettable
{
    [SerializeField] PlayerEvents _playerEvents;
    private ParticleController leftFire, centreFire, rightFire;
    private Animator anim;
    private Collider2D collider2D;
    public void Start()
    {
        leftFire = transform.GetChild(0).GetComponent<ParticleController>();
        centreFire = transform.GetChild(1).GetComponent<ParticleController>();
        rightFire = transform.GetChild(2).GetComponent<ParticleController>();

        anim = GetComponent<Animator>();

        collider2D = GetComponent<BoxCollider2D>();

        _playerEvents.onPlayerRespawn += ResetSelf;

    }
    public void DisableSelf()
    {
        leftFire.StopEmission();
        centreFire.StopEmission();
        rightFire.StopEmission();

        collider2D.enabled = false;
    }
    public void ResetSelf()
    {
        anim.SetBool("burned", false);
        collider2D.enabled = true;
    }
    private void StartBurn()
    {
        anim.SetBool("burned", true);

        leftFire.StartEmission();
        centreFire.StartEmission();
        rightFire.StartEmission();

    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartBurn();
        }
    }

}
