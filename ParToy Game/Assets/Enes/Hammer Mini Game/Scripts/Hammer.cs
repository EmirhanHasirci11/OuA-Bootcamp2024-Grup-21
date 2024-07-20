using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Hammer : NetworkBehaviour
{
    // references
    private Camera mainCamera;
    private Rigidbody rb;
    public BarManager remainingTimeBar;
    public PlayerHealth playerHealth; // reference where the playerId is
    private float remainingDragTime; // drag time

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
    public NetworkVariable<int> hammerId = new NetworkVariable<int>();


    void Start()
    {
        // initialization
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        remainingDragTime = maxDragTime;

        if (IsOwner)
        {
            hammerId.Value = Random.Range(1, 5000);
            playerHealth.playerId.Value = hammerId.Value;
        }
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
    }

    void increaseDragTime()
    {
        if (remainingDragTime / maxDragTime < 1)
        {
            remainingDragTime += Time.deltaTime;
            remainingTimeBar.SetTimeBar(remainingDragTime / maxDragTime);
        }
    }

    void Update()
    {
        if (!IsOwner) return;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Disable dragging for 2 seconds when collided
            isDisabled = true;

            // compute the dealing damage
            float hitVelocity = GetComponent<Rigidbody>().velocity.magnitude;
            float hitDamage = hammerPower * hitVelocity;

            // get the collided player as PlayerHealth and deal damage
            PlayerHealth otherPlayer = collision.gameObject.GetComponent<PlayerHealth>();

            if (otherPlayer != null && otherPlayer.playerId.Value != hammerId.Value && !isAttacked && hitVelocity > 8)
            {
                Debug.Log(hammerId.Value);
                Debug.Log(otherPlayer.playerId.Value);

                Debug.Log(otherPlayer.playerId.Value != hammerId.Value);

                otherPlayer.TakeDamageServerRpc((int)hitDamage);

                isAttacked = true;
                Debug.Log(otherPlayer.currentHealth.Value);
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