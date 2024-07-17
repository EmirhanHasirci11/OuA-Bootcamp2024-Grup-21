using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMove : MonoBehaviour
{
    public Transform target; // Takip edilecek hedef (karakteriniz)
    public float distance = 5.0f; // Hedefe olan mesafe
    public float heightOffset = 1.5f; // Yükseklik ofseti
    public float rotationSpeed = 5.0f; // Dönüþ hýzý
    public float minVerticalAngle = -45.0f; // Minimum dikey açý
    public float maxVerticalAngle = 45.0f; // Maksimum dikey açý

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Ýmleci gizle ve ortala
    }

    void LateUpdate()
    {
        if (target == null) return; // Hedef yoksa çýk

        // Fare hareketini al
        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Dikey açýyý sýnýrla
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        // Kameranýn dönüþünü hesapla
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // Kameranýn pozisyonunu hesapla
        Vector3 position = target.position - (rotation * Vector3.forward * distance) + Vector3.up * heightOffset;

        // Kamerayý ayarla
        transform.position = position;
        transform.LookAt(target.position + Vector3.up * heightOffset); // Hedefe bak
    }
}
