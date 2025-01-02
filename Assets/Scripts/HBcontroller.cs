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

        // Eğer kontrol ediliyorsa fizik motorunu etkin tut
        if (isControlled)
        {
            hb.isKinematic = false; // Hoverboard hareket edebilir
        }
        else
        {
            hb.isKinematic = true; // Hoverboard'u sabitle
            moveInput = Vector2.zero; // Hareket girişlerini sıfırla
        }
    }

    // Hoverboard'a binme için çağrılacak fonksiyon
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Karakter Hoverboard'a yaklaştı.");
        }
    }

    // Hoverboard'dan inme işlemi için
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Karakter Hoverboard'dan uzaklaştı.");
        }
    }
}









