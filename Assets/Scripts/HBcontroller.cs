using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HBcontroller : MonoBehaviour
{
    private Rigidbody hb;

    [Header("Hoverboard Settings")]
    public float mult = 10f; // Yükselme kuvveti çarpaný
    public float moveForce = 500f; // Ýleri hareket kuvveti
    public float turnTorque = 200f; // Dönüþ kuvveti

    [Header("Stabilization Settings")]
    public float targetHeight = 0.5f; // Hoverboard'un hedef yüksekliði (yerden uzaklýk)
    public float dampingFactor = 0.5f; // Stabilizasyon sönümleme faktörü (yavaþlatma)
    public float maxForce = 20f; // Kuvvetin maksimum limiti

    [Header("Input Settings")]
    public InputActionReference moveControl; // Hareket input'u

    public Transform[] anchors = new Transform[4]; // Hoverboard'un raycast noktalarý
    private RaycastHit[] hits = new RaycastHit[4]; // Raycast sonuçlarý

    private Vector2 moveInput; // Input System'den gelen hareket vektörü

    void Start()
    {
        hb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        moveControl.action.Enable();
        moveControl.action.performed += OnMove;
        moveControl.action.canceled += OnMove;
    }

    private void OnDisable()
    {
        moveControl.action.Disable();
        moveControl.action.performed -= OnMove;
        moveControl.action.canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {        
        for (int i = 0; i < anchors.Length; i++)
        {
            ApplyForce(anchors[i], hits[i]);
        }

        
        hb.AddForce(moveInput.y * moveForce * transform.forward, ForceMode.Force);
        
        hb.AddTorque(moveInput.x * turnTorque * transform.up, ForceMode.Force);
    }

    void ApplyForce(Transform anchor, RaycastHit hit)
    {
        Vector3 rayDirection = -anchor.up; 
        Debug.DrawRay(anchor.position, rayDirection * 2f, Color.red); 

        if (Physics.Raycast(anchor.position, rayDirection, out hit, 2f)) 
        {
            float distanceToGround = hit.distance;
            
            float distanceError = targetHeight - distanceToGround; 
            
            float stabilizationForce = distanceError * mult;
            
            float dampingForce = -hb.GetPointVelocity(anchor.position).y * dampingFactor;
            
            float totalForce = stabilizationForce + dampingForce;
            
            totalForce = Mathf.Clamp(totalForce, -maxForce, maxForce);
            
            hb.AddForceAtPosition(transform.up * totalForce, anchor.position, ForceMode.Acceleration);
        }
        else
        {
            Debug.LogWarning($"Anchor {anchor.name} - Raycast did not hit anything!");
        }
    }
}

