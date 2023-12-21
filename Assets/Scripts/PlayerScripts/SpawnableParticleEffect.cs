using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableParticleEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] AudioSource _audioSource;

    private void Start()
    {
        _audioSource.Play();
        _particleSystem.Play();
        StartCoroutine(ExplosionLifeTime());
    }

    IEnumerator ExplosionLifeTime()
    {
        yield return new WaitForSeconds(_audioSource.clip.length);
        Destroy(gameObject);
    }
}
