using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.SceneView;
using UnityEngine.InputSystem.XR;

public class TestCharacter : MonoBehaviour
{
    // references
    private PlayerInput playerInput;
    private CharacterController characterController;
    private Animator animator;
    public Transform Cam;
    public Camera camera;

    // input values
    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;
    private bool isMovementPressed;
    private bool isRunPressed;

    // constants
    public float movementSpeed = 10.0f;
    public float runSpeed = 30.0f;
    public float rotationSmoothTime = 0.12f; // Düzgün dönüþ için
    private float turnSmoothVelocity; // Düzgün dönüþ için

    private float groundedGravity = -0.05f;
    private float gravity = -9.8f;

    // jump variables
    private bool isJumpPressed = false;
    private float initialJumpVelocity;
    private float maxJumpHeight = 10.0f;
    private float maxJumpTime = 0.5f;
    private bool isJumping = false;


    private void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Hareket 
        playerInput.CharacterControls.Movement.performed += context =>
        {
            currentMovementInput = context.ReadValue<Vector2>();
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        };
        playerInput.CharacterControls.Movement.canceled += context =>
        {
            currentMovementInput = Vector2.zero;
            isMovementPressed = false;
        };

        // Koþma
        playerInput.CharacterControls.Run.performed += context => { isRunPressed = context.ReadValueAsButton(); };
        playerInput.CharacterControls.Run.canceled += context => { isRunPressed = context.ReadValueAsButton(); };

        // Zýplama
        playerInput.CharacterControls.Jump.started += context => { isJumpPressed = context.ReadValueAsButton(); };
        playerInput.CharacterControls.Jump.canceled += context => { isJumpPressed = context.ReadValueAsButton(); };

        // Zýplama deðiþkenlerini ayarla
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void Update()
    {
        handleRotation();
        handleAnimation();
        handleMove();
        handleGravity();
        handleJump();
    }

    void handleGravity()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else
        {
            currentMovement.y += gravity * Time.deltaTime;
            currentRunMovement.y += gravity * Time.deltaTime;
        }
    }

    void handleMove()
    {
        // Kameranýn baktýðý yöne göre hareketi hesapla
        Vector3 direction = new Vector3(currentMovementInput.x, 0f, currentMovementInput.y).normalized;
        Vector3 moveDir = camera.transform.forward * direction.z + camera.transform.right * direction.x;
        moveDir.y = 0f;

        // Hýzlarý ayarla
        currentMovement = moveDir * movementSpeed;
        currentRunMovement = moveDir * runSpeed;

        // Hareket ettir
        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }
    }

    void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            isJumping = true;
            currentMovement.y = initialJumpVelocity;
            currentRunMovement.y = initialJumpVelocity;
        }
        else if (!isJumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        // Sadece hareket varsa döndür
        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);

            // Düzgün bir þekilde döndür
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
        }
    }

    void handleAnimation()
    {
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool("isWalking", true);
        }
        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool("isWalking", false);
        }
        if ((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool("isRunning", true);
        }
        else if ((!isMovementPressed || !isRunPressed) && isRunning)
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