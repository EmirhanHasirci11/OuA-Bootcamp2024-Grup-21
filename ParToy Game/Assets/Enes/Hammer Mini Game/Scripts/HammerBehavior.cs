using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HammerBehavior : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoord;
    private Rigidbody rb;
    private float forceMultiplier = 7f;
    private float hammerPower = 0.2f;
    private bool isAttacked = false;
    private bool isDisabled = false;
    private bool isDragging = false;
    private float disableTime = 1.5f;
    private float maxDragTime = 5f;
    private float remainingDragTime;
    public RemainingTimeBar remainingTimeBar;
    

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        remainingDragTime = maxDragTime;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                isDragging = true;
                zCoord = mainCamera.WorldToScreenPoint(transform.position).z;
                offset = transform.position - GetMouseWorldPos();
            }
        }

        if (isDragging && remainingDragTime > 0)
        {
            if (!isDisabled)
            {
                decreaseDragTime();

                Vector3 newPos = GetMouseWorldPos() + offset;
                Vector3 force = (newPos - transform.position) * forceMultiplier;
                rb.AddForce(force, ForceMode.Force);
            }


        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (!isDragging && !isDisabled)
        {
            increaseDragTime();
        }
    }


    void decreaseDragTime()
    {
        if (remainingDragTime > 0)
        {
            remainingDragTime -= Time.deltaTime;
            remainingTimeBar.SetTimeBar(remainingDragTime / maxDragTime);
        }
    }

    void increaseDragTime()
    {
        if (remainingDragTime / maxDragTime < 1)
        {
            remainingDragTime += Time.deltaTime;
            remainingTimeBar.SetTimeBar(remainingDragTime / maxDragTime);
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hasar verildi, 2 saniye çekicin kontrolü kaybedildi.");
            // Disable dragging for 2 seconds when collided
            isDisabled = true;

            // Deal damage
            float hitVelocity = GetComponent<Rigidbody>().velocity.magnitude;
            float hitDamage = hammerPower * hitVelocity;

            if (!isAttacked && hitVelocity > 8)
            {
                Debug.Log(rb.velocity.magnitude);

                // Prevent double collisions from happening
                isAttacked = true;
            }

            Invoke(nameof(resetEnable), 2f);
        }
    }

    void resetEnable()
    {
        isDisabled = false;
        isAttacked = false;
    }


}
