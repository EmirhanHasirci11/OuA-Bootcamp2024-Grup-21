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
        // Sadece yerel oynatýcý için imleci kilitle ve kamerayý etkinleþtir
        if (IsLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            GetComponent<Camera>().enabled = true;
        }
    }

    void LateUpdate()
    {
        // Sadece yerel oynatýcýyý takip et
        if (!IsLocalPlayer || target == null) return;

        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        // Rotasyon ve pozisyon hesaplama:
        Quaternion targetRotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 targetPosition = target.position - (targetRotation * Vector3.forward * distance) + Vector3.up * heightOffset;

        // Pozisyonu ve rotasyonu doðrudan ayarla:
        transform.rotation = targetRotation; // Düzgünleþtirme yok
        transform.position = targetPosition; // Düzgünleþtirme yok
    }
}