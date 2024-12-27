using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HBcontroller : MonoBehaviour
{
    private Rigidbody hb;

    [Header("Hoverboard Settings")]
    public float mult = 10f; // Y�kselme kuvveti �arpan�
    public float moveForce = 500f; // �leri hareket kuvveti
    public float turnTorque = 200f; // D�n�� kuvveti

    [Header("Stabilization Settings")]
    public float targetHeight = 0.5f; // Hoverboard'un hedef y�ksekli�i (yerden uzakl�k)
    public float dampingFactor = 0.5f; // Stabilizasyon s�n�mleme fakt�r� (yava�latma)
    public float maxForce = 20f; // Kuvvetin maksimum limiti

    [Header("Input Settings")]
    public InputActionReference moveControl; // Hareket input'u

    public Transform[] anchors = new Transform[4]; // Hoverboard'un raycast noktalar�
    private RaycastHit[] hits = new RaycastHit[4]; // Raycast sonu�lar�

    private Vector2 moveInput; // Input System'den gelen hareket vekt�r�

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
        // Her bir anchor i�in kuvvet uygula
        for (int i = 0; i < anchors.Length; i++)
        {
            ApplyForce(anchors[i], hits[i]);
        }

        // �leri hareket
        hb.AddForce(moveInput.y * moveForce * transform.forward, ForceMode.Force);

        // D�n��
        hb.AddTorque(moveInput.x * turnTorque * transform.up, ForceMode.Force);
    }

    void ApplyForce(Transform anchor, RaycastHit hit)
    {
        Vector3 rayDirection = -anchor.up; // Ray'in y�n�
        Debug.DrawRay(anchor.position, rayDirection * 2f, Color.red); // Ray'i g�rselle�tir

        if (Physics.Raycast(anchor.position, rayDirection, out hit, 2f)) // Maksimum mesafe ayarla
        {
            float distanceToGround = hit.distance;

            // Hoverboard'un hedef y�ksekli�i ve stabilizasyon kuvveti
            float distanceError = targetHeight - distanceToGround; // Hedef y�kseklik fark�

            // Stabilizasyon kuvveti (Proportional)
            float stabilizationForce = distanceError * mult;

            // S�n�mleme kuvveti (Damping)
            float dampingForce = -hb.GetPointVelocity(anchor.position).y * dampingFactor;

            // Kuvveti birle�tir
            float totalForce = stabilizationForce + dampingForce;

            // Kuvvetin a��r� olmas�n� engelle
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

