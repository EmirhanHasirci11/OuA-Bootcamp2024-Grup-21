using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedIsometricCharacterController : MonoBehaviour
{
    // references
    private PlayerInput playerInput;
    private CharacterController characterController;
    private Animator animator;

    // input values
    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;
    private bool isMovementPressed;
    private bool isRunPressed;

    // constants
    private float movementSpeed = 5.0f;
    private float runSpeed = 8.0f;
    private float rotationFactorPerFrame = 15.0f;
    private float groundedGravity = -.05f;
    private float gravity = -9.8f;

    // jump variables
    private bool isJumpPressed = false;
    private float initialJumpVelocity;
    private float maxJumpHeight = 1.0f;
    private float maxJumpTime = 0.8f;
    private bool isJumping = false;
    private int jumpCount = 0;
    private int maxJumps = 2;   

    private void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // move part
        playerInput.CharacterControls.Movement.performed += context => {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x * movementSpeed;
            currentMovement.z = currentMovementInput.y * movementSpeed;
            currentRunMovement.x = currentMovementInput.x * runSpeed;
            currentRunMovement.z = currentMovementInput.y * runSpeed;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        };

        // to stop the character when the buttons are released
        playerInput.CharacterControls.Movement.canceled += context => {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        };

        // run part
        playerInput.CharacterControls.Run.performed += context => { isRunPressed = context.ReadValueAsButton(); };
        playerInput.CharacterControls.Run.canceled += context => { isRunPressed = context.ReadValueAsButton(); };

        // jump part
        playerInput.CharacterControls.Jump.started += context => { isJumpPressed = context.ReadValueAsButton(); };
        playerInput.CharacterControls.Jump.canceled += context => { isJumpPressed = context.ReadValueAsButton(); };

        //setup jump variables
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;

    }

    void handleGravity()
    {
        if (characterController.isGrounded)
        {
            animator.SetBool("isJumping", false);
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
        if (characterController.isGrounded)
        {
            // Reset the jump count and isJumping when the character is grounded
            jumpCount = 0;
            isJumping = false;
            animator.SetBool("isJumping", false);
        }

        if (isJumpPressed && (characterController.isGrounded || jumpCount < maxJumps))
        {
            animator.SetBool("isJumping", true);
            isJumping = true;
            currentMovement.y = initialJumpVelocity;
            currentRunMovement.y = initialJumpVelocity;
            jumpCount++;
            isJumpPressed = false; // Ensure this is only triggered once per jump press
        }

        if (!isJumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
            animator.SetBool("isJumping", false);
        }
    }

    void handleRotation()
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
        //jump i?in ekleme yap?lacak !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    void Update()
    {
        handleRotation();
        handleAnimation();
        handleMove();
        handleGravity();
        handleJump();
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