using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    // references
    private Camera mainCamera;
    private Rigidbody rb;
    public BarManager remainingTimeBar;
    public PlayerHealth playerHealth; // reference where the playerId is
    private float remainingDragTime; // drag time
    public GameObject origin; // origin of the circle that hammer is going to move (player object)

    // variables that will be used to find mouse position
    private Vector3 offset;
    private float zCoord;

    // constants
    private float forceMultiplier = 7f;
    private float hammerPower = 0.2f;
    private float disableTime = 2f;
    private float maxDragTime = 5f;

    // booleans
    private bool isAttacked = false;
    private bool isDisabled = false;
    private bool isDragging = false;

    // unique hammer id
    public int hammerId = 0;

    void Start()
    {
        // initialization
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        remainingDragTime = maxDragTime;

        // randomly initialize player id and match it with the hammer that player uses
        hammerId = UnityEngine.Random.Range(1, 5000);
        playerHealth.playerId = hammerId;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    void decreaseDragTime()
    {
        if (remainingDragTime > 0)
        {
            remainingDragTime -= Time.deltaTime;
            remainingTimeBar.SetTimeBar(remainingDragTime / maxDragTime);
        }

        // disable hammer for 2 seconds when the drag time is fully used
        if (!isDisabled && remainingDragTime < 0)
        {
            isDisabled = true;
            Invoke(nameof(resetEnable), 2f);
        }
    }

    void increaseDragTime()
    {
        if (!isDisabled && remainingDragTime / maxDragTime < 1)
        {
            remainingDragTime += Time.deltaTime;
            remainingTimeBar.SetTimeBar(remainingDragTime / maxDragTime);
        }
    }

    void handleDrag()
    {
        // get mouse position
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

        // move hammer
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

    // the force to limit hammer's maximum distance to player
    void restoringForce()
    {
        Vector3 hammerPosition = this.transform.position;
        Vector3 originPosition = origin.transform.position;
        float restoringForceMultiplier = 5;

        if (isDragging && Vector3.Distance(hammerPosition, originPosition) > 5)
        {
            rb.AddForce((originPosition - hammerPosition) * rb.velocity.magnitude * restoringForceMultiplier, ForceMode.Force);
        }

        // decrease velocity when the hammer is not being dragged
        if (isDisabled || (!isDragging && rb.velocity.magnitude > 3))
        {
            rb.velocity *= 0.9f;
        }
    }

    void Update()
    {
        handleDrag();
        restoringForce();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Disable dragging for 2 seconds when collided
            isDisabled = true;

            // compute dealing damage
            float hitVelocity = GetComponent<Rigidbody>().velocity.magnitude;
            float hitDamage = hammerPower * hitVelocity;

            // get the collided player as PlayerHealth and deal damage
            PlayerHealth otherPlayer = collision.gameObject.GetComponent<PlayerHealth>();
            if (otherPlayer != null && otherPlayer.playerId != hammerId && !isAttacked && hitVelocity > 8)
            {
                otherPlayer.TakeDamage(((int)hitDamage));
                isAttacked = true;
            }

            Invoke(nameof(resetEnable), disableTime);
        }
    }

    void resetEnable()
    {
        isDisabled = false;
        isAttacked = false;
    }
}
