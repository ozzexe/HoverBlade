using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followPosition;
    public Transform targetToLookAt;

    public float smoothSpeed = 0.125f;

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, followPosition.position, smoothSpeed);
        transform.LookAt(targetToLookAt);
    }
}
