using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSpirit : MonoBehaviour
{
    [SerializeField] private float playerDetectionRadius;
    [SerializeField] private float _pullStrength;
    [SerializeField] private float _escapeSpeed;
    [SerializeField] private LayerMask playerLayer;

    private Rigidbody2D _playerRb;


    private void Update()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, playerDetectionRadius, playerLayer);
        // Check For player
        if (collider != null && _playerRb == null)
        {
            Debug.Log("Player in Range");


            PlayerController player;
            if (collider.TryGetComponent<PlayerController>(out player))
            {
                _playerRb = player.GetComponent<Rigidbody2D>();
                _playerRb.gravityScale = 0;
                _playerRb.drag = 0.84f;
            }
        }
        else if (collider == null && _playerRb != null)
        {
            Debug.Log("Player Not In Range");
            _playerRb.gravityScale = 1;
            _playerRb.drag = 0;
            _playerRb = null;
        }


    }
    void FixedUpdate()
    {
        //if (Physics2D.OverlapCircle(transform.position, playerDetectionRadius, playerLayer))
        //{
        //    GameEvents.current.onApplyForceToPlayer(transform.position, -suctionMultiplier);
        //}
      
        if (_playerRb != null)
        {
            Debug.Log("player In Range of Air Spirit");

            // Create tractor beam effect

            // If under a certain speed exert a heigher force on player, but don't accelerate them super fast

            // Over that speed reduce traction speed


            
            

            Vector2 dir = _playerRb.position - new Vector2(transform.position.x, transform.position.y);
            float radius = dir.magnitude;

            float acceleration;

            if (_playerRb.velocity.magnitude < _escapeSpeed)
            {
                acceleration = Mathf.Pow(_pullStrength, 1.1f) / Mathf.Max(1, radius);

            }
            else
            {
                acceleration = Mathf.Pow(_pullStrength, 1) / Mathf.Max(1, radius);
            }

            _playerRb.AddForce(-dir.normalized * acceleration);            
        }
    }   

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }

}
