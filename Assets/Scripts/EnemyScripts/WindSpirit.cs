using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSpirit : MonoBehaviour
{
    [SerializeField] private float playerDetectionRadius;
    [SerializeField] private float suctionMultiplier;
    [SerializeField] private LayerMask playerLayer;
    
    void FixedUpdate()
    {
        if (Physics2D.OverlapCircle(transform.position, playerDetectionRadius, playerLayer))
        {
            GameEvents.current.onApplyForceToPlayer(transform.position, -suctionMultiplier);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }

}
