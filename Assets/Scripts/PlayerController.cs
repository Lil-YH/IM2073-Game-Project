using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float baseSpeed = 2.0f;
    [SerializeField]
    private float sprintSpeed = 3.0f;
    [SerializeField]
    public float aimSpeed = 1.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = .8f;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform barrelTransform;
    [SerializeField]
    private Transform bulletParent;
    [SerializeField]
    private float bulletHitMissDistance = 25f;
    [SerializeField]
    private float animationSmoothTime = .1f;
    [SerializeField]
    private float animationPlayTransition = .15f;
    [SerializeField]
    public float playerMaxHealth = 50.0f;

    [HideInInspector]
    public float playerCurrentHealth;

    [SerializeField]
    public float damage = 20;
    // Timer to track collision time
    float _timeColliding = 2f;
    // Time before damage is taken, 1 second default
    public float timeThreshold = 0.5f;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;
    private InputAction sprintAction;
    private Transform cameraTransform;
    private Animator animator;
    public TrailRenderer tracerEffect;
    public ParticleSystem muzzleFlash;
    public AudioSource gunShotSource;
    public AudioClip gunShotClip;
    public AudioClip damageClip;
    public CinemachineVirtualCamera aimCamera;
    public CinemachineVirtualCamera normalCamera;
    public GameManager gameManager;
    [SerializeField]
    private Canvas aimCanvas;
    [SerializeField]
    private Canvas thirdPersonCanvas;
    int moveXAnimationParameterId;
    int moveZAnimationParameterId;
    int jumpAnimation;
    bool isSprint;
    bool playerIsDead = false;
    public float playerSpeed;

    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
        sprintAction = playerInput.actions["Sprint"];
        Cursor.lockState = CursorLockMode.Locked;
        //Animations
        animator = GetComponent<Animator>();
        jumpAnimation = Animator.StringToHash("Jump Up");
        moveXAnimationParameterId = Animator.StringToHash("MoveX");
        moveZAnimationParameterId = Animator.StringToHash("MoveZ");
        playerSpeed = baseSpeed;

        playerCurrentHealth = playerMaxHealth;
    }

    private void OnEnable()
    {
        shootAction.performed += _ => ShootGun();
        sprintAction.performed += _ => StartSprint();
        sprintAction.canceled += _ => CancelSprint();
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => ShootGun();
        sprintAction.performed += _ => StartSprint();
        sprintAction.canceled += _ => CancelSprint();
    }

    private void StartSprint()
    {
        playerSpeed = sprintSpeed;
        isSprint = true;
       
    }
    private void CancelSprint()
    {
        playerSpeed = baseSpeed;
        isSprint = false;
        
    }

    private void ShootGun()
    {
        RaycastHit hit;
        GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        if(!PauseMenu.GameIsPaused && !GameManager.GameIsComplete && !GameManager.GameIsOver)
        {
            muzzleFlash.Emit(1);
            gunShotSource.PlayOneShot(gunShotClip);
            var tracer = Instantiate(tracerEffect, barrelTransform.position, Quaternion.identity);
            tracer.AddPosition(barrelTransform.position);
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
            {
                bulletController.target = hit.point;
                tracer.transform.position = hit.point;
                bulletController.hit = true;
            }
            else
            {
                bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;
                bulletController.hit = false;
            }

            var hitBox = hit.collider.GetComponent<HitBox>();
            if(hitBox)
            {
                hitBox.OnRaycastHit(this, bulletController.target);
            }
        }
        
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        _timeColliding += Time.deltaTime;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime);
        Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);
       
        //Blend Animation
        animator.SetFloat(moveXAnimationParameterId, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParameterId, currentAnimationBlendVector.y);

        // Changes the height position of the player..
        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            animator.CrossFade(jumpAnimation, animationPlayTransition);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        //Rotate towards camers direction

        float targetAngle = cameraTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // player is dead = Game Over
        if(playerCurrentHealth <= 0)
        {
            playerSpeed = 0.0f;
            playerIsDead = true;

            if(playerIsDead && !GameManager.GameIsOver)
            {
                gameManager.GameOver();
            }
        }

        // update cursor state
        if (PauseMenu.GameIsPaused || GameManager.GameIsComplete || GameManager.GameIsOver)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            aimCanvas.enabled = false;
            thirdPersonCanvas.enabled = false;
        } else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //aimCanvas.enabled = true;
            //thirdPersonCanvas.enabled = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11)
        {
            
            if (_timeColliding > timeThreshold)
            {
                // Time is over theshold, player takes damage
                playerCurrentHealth -= 5.0f;
                gunShotSource.PlayOneShot(damageClip);
                // Reset timer
                _timeColliding = 0f;
            }
           
        }
    }
   
}