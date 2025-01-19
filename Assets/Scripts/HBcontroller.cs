using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HBcontroller : MonoBehaviour
{
    private Rigidbody hb;

    [Header("Hoverboard Settings")]
    public float mult = 10f;
    public float moveForce = 500f;
    public float turnTorque = 200f;

    [Header("Stabilization Settings")]
    public float targetHeight = 0.5f;
    public float dampingFactor = 0.5f;
    public float maxForce = 20f;

    [Header("Input Settings")]
    public InputActionReference moveControl;

    public Transform[] anchors = new Transform[5];
    private RaycastHit[] hits = new RaycastHit[5];

    private Vector2 moveInput;
    private bool isControlled = false; // Hoverboard kontrol durumunu takip eder

    [Header("Call Settings")]
    public float callSpeed = 5f; // Hoverboard'un çağrıldığında hareket hızı
    private bool isBeingCalled = false; // Hoverboard'un çağrıldığı durumu kontrol eder
    private Vector3 callTargetPosition; // Hoverboard'un hareket edeceği hedef pozisyon

    void Start()
    {
        hb = GetComponent<Rigidbody>();
    }

    public void Mount()
    {
        moveControl.action.Enable();
        moveControl.action.performed += OnMove;
        moveControl.action.canceled += OnMove;
    }

    public void Dismount()
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
        if (isBeingCalled)
        {
            // Hoverboard'u hedef pozisyona taşır
            MoveToCallTarget();
            return;
        }

        // Hoverboard sadece kontrol edildiğinde çalışır
        if (!isControlled) return;

        // Stabilizasyon kuvvetlerini uygula
        for (int i = 0; i < anchors.Length; i++)
        {
            ApplyForce(anchors[i], hits[i]);
        }

        // Hareket ve dönüş kuvvetlerini uygula
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
    }

    // Hoverboard kontrolünü aç/kapat
    public void SetControlled(bool controlStatus)
    {
        isControlled = controlStatus;

        if (isControlled)
        {
            isBeingCalled = false; // Çağrılma durumunu iptal et
            hb.isKinematic = false; // Hoverboard hareket edebilir
        }
        else
        {
            hb.isKinematic = true; // Hoverboard'u sabitle
            moveInput = Vector2.zero; // Hareket girişlerini sıfırla
        }
    }

    // Hoverboard'u belirli bir pozisyona çağır
    public void CallToPosition(Vector3 targetPosition)
    {
        isBeingCalled = true;
        callTargetPosition = targetPosition;
        hb.isKinematic = false; // Hareket edebilmesi için fizik motorunu etkinleştir
    }

    // Hoverboard'u çağırma hedef pozisyonuna taşır
    private void MoveToCallTarget()
    {
        Vector3 direction = (callTargetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, callTargetPosition);

        if (distance > 0.1f)
        {
            // Hedef pozisyona doğru hareket et
            hb.linearVelocity = direction * callSpeed;
        }
        else
        {
            // Hedefe ulaşıldıysa hareketi durdur
            hb.linearVelocity = Vector3.zero;
            isBeingCalled = false;
            hb.isKinematic = true; // Hedefte durunca sabitlenir
        }
    }
}

