using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoverboard : MonoBehaviour
{
    // Initialise Variables
    Rigidbody rb;
    public float moveForce, turnForce, multiplier;

    public Transform[] anchors = new Transform[4];
    RaycastHit[] hits = new RaycastHit[4];
    public LayerMask canFloatOn;

    public ParticleSystem[] jets;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per fram
    void Update()
    {
        // Jets turns on when button is pushed
        if (Input.GetButtonDown("Vertical"))
        {
            for (int i = 0; i < 2; i++)
            {
                jets[i].Play();
            }
        }
        // Jets turns off when button is let go
        if (Input.GetButtonUp("Vertical"))
        {
            for (int i = 0; i < 2; i++)
            {
                jets[i].Stop();
            }
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < 4; i++)
        {       // Each anchor sens a raycast down
            if (Physics.Raycast(anchors[i].position, -anchors[i].up, out hits[i], Mathf.Infinity, canFloatOn))
            {
                // Anchor pushes upwards if raycast hits something on 'ground' layer
                float anchorForce = Mathf.Abs(1 / (hits[i].point.y - anchors[i].position.y));
                rb.AddForceAtPosition(transform.up * anchorForce * multiplier, anchors[i].position, ForceMode.Acceleration);
            }
        }

        // Basic movement controls
        rb.AddForce(Input.GetAxis("Vertical") * moveForce * transform.forward);
        rb.AddTorque(Input.GetAxis("Horizontal") * turnForce * transform.up);
    }
}
