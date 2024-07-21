using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileConquestCharController : MonoBehaviour
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
    private float movementSpeed = 20.0f;
    private float runSpeed = 30.0f;
    private float rotationFactorPerFrame = 15.0f;
    private float groundedGravity = -.05f;
    private float gravity = -9.8f;

    // jump variables
    private bool isJumpPressed = false;
    private float initialJumpVelocity;
    private float maxJumpHeight = 3.0f;
    private float maxJumpTime = 0.5f;
    private bool isJumping = false;

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
        // Yerçekimini her zaman uygula
        currentMovement.y += gravity * Time.deltaTime;
        currentRunMovement.y += gravity * Time.deltaTime;

        // Animasyon için yere düþüp düþmediðini kontrol et
        if (characterController.isGrounded && animator.GetBool("isJumping"))
        {
            animator.SetBool("isJumping", false);
        }
    }

    void handleMove()
    {
        // Kameranýn baktýðý yöne göre hareketi hesapla
        Vector3 direction = new Vector3(currentMovementInput.x, 0f, currentMovementInput.y).normalized;
        Vector3 moveDir = camera.transform.forward * direction.z + camera.transform.right * direction.x;

        // Hýzlarý ayarla (y eksenini koruyarak)
        currentMovement.x = moveDir.x * movementSpeed;
        currentMovement.z = moveDir.z * movementSpeed;
        currentRunMovement.x = moveDir.x * runSpeed;
        currentRunMovement.z = moveDir.z * runSpeed;

        // Hareket ettir (gravity handleGravity() fonksiyonunda uygulanacak)
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
            animator.SetBool("isJumping", true);
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