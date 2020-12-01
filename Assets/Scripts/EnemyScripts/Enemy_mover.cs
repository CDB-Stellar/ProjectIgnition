using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class Enemy_mover : MonoBehaviour, IResettable
{
    public float health;
	public float speed;
	public bool MoveRight;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        GameEvents.current.onPlayerRespawn += ResetSelf;
    }    
    private void Update()
	{
		// Use this for initialization
		if (MoveRight)
		{
			transform.Translate(2 * Time.deltaTime * speed, 0, 0);
			transform.localScale = new Vector2(-1, 1);
		}
		else
		{
			transform.Translate(-2 * Time.deltaTime * speed, 0, 0);
			transform.localScale = new Vector2(1, 1);
		}
	}
    public void ResetSelf()
    {
        transform.position = startPos;
        gameObject.SetActive(true);
    }

    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("turn"))
		{
            MoveRight = !MoveRight;
		}
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            float damage = other.GetComponent<FireBall>().GetDamage();
            if (damage > health)            
                DisableSelf();
            
        }
	}

    
}

