using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerWithBuffer : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Range(1.0f, 10f)][SerializeField] private float cameraOffSetX = 0.5f;
    [Range(1.0f, 10f)] [SerializeField] private float cameraOffSetY = 0.5f;
  
    // Update is called once per frame
    void Update()
    {
        //check the x threshold
        if (player.position.x < transform.position.x - (0.5f * cameraOffSetX)) // Left
        {
            transform.position = new Vector3(
                player.position.x + (0.5f * cameraOffSetX),
                transform.position.y,
                transform.position.z);
        }
        else if (player.position.x > transform.position.x + (0.5f * cameraOffSetX)) // Right
        {
            transform.position = new Vector3(
                player.position.x - (0.5f * cameraOffSetX),
                transform.position.y,
                transform.position.z);
        }

        //check the y threshold
        if (player.position.y < transform.position.y - (0.5f * cameraOffSetY)) // up
        {
            transform.position = new Vector3(
                transform.position.x ,
                player.position.y + (0.5f * cameraOffSetY),
                transform.position.z);
        }
        else if (player.position.y > transform.position.y + (0.5f * cameraOffSetY)) // down
        {
            transform.position = new Vector3(
                transform.position.x ,
                player.position.y - (0.5f * cameraOffSetY),
                transform.position.z);
        }


    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(cameraOffSetX, cameraOffSetY, 0f));
    }
}
