using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ThirdPersonMove : NetworkBehaviour
{
    public Transform target; // Takip edilecek hedef (karakteriniz)
    public float distance = 5.0f;
    public float heightOffset = 1.5f;
    public float rotationSpeed = 5.0f;
    public float minVerticalAngle = -45.0f;
    public float maxVerticalAngle = 45.0f;

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    void Start()
    {
        // Sadece yerel oynat�c� i�in imleci kilitle ve kameray� etkinle�tir
        if (IsLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            GetComponent<Camera>().enabled = true;
        }
    }

    void LateUpdate()
    {
        // Sadece yerel oynat�c�y� takip et
        if (!IsLocalPlayer || target == null) return;

        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        // Rotasyon ve pozisyon hesaplama:
        Quaternion targetRotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 targetPosition = target.position - (targetRotation * Vector3.forward * distance) + Vector3.up * heightOffset;

        // Pozisyonu ve rotasyonu do�rudan ayarla:
        transform.rotation = targetRotation; // D�zg�nle�tirme yok
        transform.position = targetPosition; // D�zg�nle�tirme yok
    }
}