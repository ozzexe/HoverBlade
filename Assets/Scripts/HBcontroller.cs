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
        // Her bir anchor için kuvvet uygula
        for (int i = 0; i < anchors.Length; i++)
        {
            ApplyForce(anchors[i], hits[i]);
        }

        // Ýleri hareket
        hb.AddForce(moveInput.y * moveForce * transform.forward, ForceMode.Force);

        // Dönüþ
        hb.AddTorque(moveInput.x * turnTorque * transform.up, ForceMode.Force);
    }

    void ApplyForce(Transform anchor, RaycastHit hit)
    {
        Vector3 rayDirection = -anchor.up; // Ray'in yönü
        Debug.DrawRay(anchor.position, rayDirection * 2f, Color.red); // Ray'i görselleþtir

        if (Physics.Raycast(anchor.position, rayDirection, out hit, 2f)) // Maksimum mesafe ayarla
        {
            float distanceToGround = hit.distance;

            // Hoverboard'un hedef yüksekliði ve stabilizasyon kuvveti
            float distanceError = targetHeight - distanceToGround; // Hedef yükseklik farký

            // Stabilizasyon kuvveti (Proportional)
            float stabilizationForce = distanceError * mult;

            // Sönümleme kuvveti (Damping)
            float dampingForce = -hb.GetPointVelocity(anchor.position).y * dampingFactor;

            // Kuvveti birleþtir
            float totalForce = stabilizationForce + dampingForce;

            // Kuvvetin aþýrý olmasýný engelle
            totalForce = Mathf.Clamp(totalForce, -maxForce, maxForce);

            // Kuvveti uygulama
            hb.AddForceAtPosition(transform.up * totalForce, anchor.position, ForceMode.Acceleration);
        }
        else
        {
            Debug.LogWarning($"Anchor {anchor.name} - Raycast did not hit anything!");
        }
    }
}

