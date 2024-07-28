using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TileConquestCharController : NetworkBehaviour
{
    // References
    private PlayerInput playerInput;
    private CharacterController characterController;
    private Animator animator;
    public Transform Cam;
    public Camera camera;

    // Input values
    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;
    private bool isMovementPressed;
    private bool isRunPressed;

    // Constants
    private float movementSpeed = 20.0f;
    private float runSpeed = 30.0f;
    private float rotationFactorPerFrame = 15.0f;
    private float groundedGravity = -0.05f;
    private float gravity = -20.0f; // Gravity value adjusted

    private float groundCheckDistance = 0.1f;

    // Jump variables
    private bool isJumpPressed = false;
    private float initialJumpVelocity;
    private float maxJumpHeight = 3.0f;
    private float maxJumpTime = 0.5f;
    private bool isJumping = false;

    // --- Network Synchronization ---
    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>(
        new PlayerState(),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private struct PlayerState : INetworkSerializable
    {
        public Vector3 position;
        public Quaternion rotation;
        public float verticalVelocity;
        public bool isJumping;
        public bool isWalking;
        public bool isRunning;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotation);
            serializer.SerializeValue(ref verticalVelocity);
            serializer.SerializeValue(ref isJumping);
            serializer.SerializeValue(ref isWalking);
            serializer.SerializeValue(ref isRunning);
        }
    }

    private PlayerState previousState;
    private float lastUpdateTime = 0f;

    // --- Unity Methods ---

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsLocalPlayer)
        {
            Cam.gameObject.SetActive(true);
            ThirdPersonMove cameraScript = GetComponentInChildren<ThirdPersonMove>();
            if (cameraScript != null)
            {
                cameraScript.target = this.transform;
            }
            else
            {
                Debug.LogWarning("ThirdPersonMove component not found on player!");
            }
        }
        else
        {
            Cam.gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Input setup 
        playerInput.CharacterControls.Movement.performed += context =>
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x * movementSpeed;
            currentMovement.z = currentMovementInput.y * movementSpeed;
            currentRunMovement.x = currentMovementInput.x * runSpeed;
            currentRunMovement.z = currentMovementInput.y * runSpeed;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        };

        playerInput.CharacterControls.Movement.canceled += context =>
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x * movementSpeed;
            currentMovement.z = currentMovementInput.y * movementSpeed;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        };

        playerInput.CharacterControls.Run.performed += context => { isRunPressed = context.ReadValueAsButton(); };
        playerInput.CharacterControls.Run.canceled += context => { isRunPressed = context.ReadValueAsButton(); };

        playerInput.CharacterControls.Jump.started += context => { isJumpPressed = context.ReadValueAsButton(); };
        playerInput.CharacterControls.Jump.canceled += context => { isJumpPressed = context.ReadValueAsButton(); };

        // Setup jump variables
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    private void Update()
    {
        if (IsOwner)
        {
            HandleInput();

            // Prepare the player state to be sent to other players
            PlayerState newState = networkPlayerState.Value;
            newState.position = transform.position;
            newState.rotation = transform.rotation;
            newState.verticalVelocity = currentMovement.y;
            newState.isJumping = isJumping;
            newState.isWalking = animator.GetBool("isWalking");
            newState.isRunning = animator.GetBool("isRunning");

            networkPlayerState.Value = newState;
        }

        if (!IsOwner)
        {
            // Interpolate for smooth movement on non-owning clients
            float interpolationRatio = (Time.time - lastUpdateTime) * 10f;
            transform.position = Vector3.Lerp(transform.position, networkPlayerState.Value.position, interpolationRatio);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkPlayerState.Value.rotation, interpolationRatio);

            // Update Animator
            animator.SetBool("isJumping", networkPlayerState.Value.isJumping);
            animator.SetBool("isWalking", networkPlayerState.Value.isWalking);
            animator.SetBool("isRunning", networkPlayerState.Value.isRunning);
        }

        lastUpdateTime = Time.time;
    }

    // ---  Movement and Input Handling  ---

    void HandleMove()
    {
        Vector3 direction = new Vector3(currentMovementInput.x, 0f, currentMovementInput.y).normalized;
        Vector3 moveDir = camera.transform.forward * direction.z + camera.transform.right * direction.x;

        currentMovement.x = moveDir.x * (isRunPressed ? runSpeed : movementSpeed);
        currentMovement.z = moveDir.z * (isRunPressed ? runSpeed : movementSpeed);

        // Apply actual movement in HandleGravity for consistency
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    private void HandleInput()
    {
        HandleRotation();
        HandleAnimation();
        HandleMove();
        HandleGravity(); // Apply Gravity consistently
        HandleJump();
    }

    void HandleGravity()
    {
        // Reliable Raycast Ground Check
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        if (isGrounded)
        {
            if (isJumping && currentMovement.y < 0)
            {
                isJumping = false;
                animator.SetBool("isJumping", false);
                currentMovement.y = groundedGravity;
            }
        }
        else
        {
            currentMovement.y += gravity * Time.deltaTime;
        }

        // Apply Movement Consistently
        characterController.Move(currentMovement * Time.deltaTime);
    }

    [ServerRpc]
    private void ServerJumpRequestServerRpc()
    {
        PlayerState newState = networkPlayerState.Value;
        newState.isJumping = true;
        networkPlayerState.Value = newState;
    }

    [ServerRpc]
    private void ServerSetJumpingStateServerRpc(bool isJumping)
    {
        PlayerState newState = networkPlayerState.Value;
        newState.isJumping = isJumping;
        networkPlayerState.Value = newState;
    }

    private void HandleJump()
    {
        if (isJumpPressed && !isJumping && characterController.isGrounded)
        {
            isJumping = true;
            currentMovement.y = initialJumpVelocity;
            animator.SetBool("isJumping", true);

            if (IsServer)
            {
                PlayerState newState = networkPlayerState.Value;
                newState.isJumping = true;
                newState.verticalVelocity = currentMovement.y;
                networkPlayerState.Value = newState;
            }
            else
            {
                ServerSetJumpingStateServerRpc(true);
            }
        }
        isJumpPressed = false;
    }

    private void HandleAnimation()
    {
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");

        if (currentMovementInput.magnitude > 0 && !isWalking)
        {
            animator.SetBool("isWalking", true);
        }
        else if (currentMovementInput.magnitude == 0 && isWalking)
        {
            animator.SetBool("isWalking", false);
        }

        if (currentMovementInput.magnitude > 0 && isRunPressed && !isRunning)
        {
            animator.SetBool("isRunning", true);
        }
        else if ((currentMovementInput.magnitude == 0 || !isRunPressed) && isRunning)
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
