using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Private Varibles
    [SerializeField] private Camera cam;

    [SerializeField] private float speed;
    [SerializeField] private Transform flameJet;
    private Rigidbody2D rbody;
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Mouse 0 value: " + Input.GetAxis("Mouse 0"));


        if (Input.GetAxis("Mouse 0") > 0.0f)
        {
            rbody.AddForce(GetJetDirection() * speed);
        }
        PointJet(GetJetDirection());
    }
    private Vector2 GetJetDirection()
    {
        Vector2 jetDir = transform.position - cam.ScreenToWorldPoint(Input.mousePosition);
        //Debug.DrawRay(transform.position, jetDir.normalized, Color.white);
        return jetDir.normalized;          
    }
    private void PointJet(Vector2 dir)
    {
        flameJet.eulerAngles = Vector3.forward * (Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg);
    }
   
}
