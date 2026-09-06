



using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementFixed : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float rotationSpeed = 180f;

    [Header("Camera Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Effects")]
    public ParticleSystem jumpStarParticles;

    [Header("Pickup & Throw")]
    public Transform holdPoint;
    public float pickupRadius = 2.5f;
    public float throwSpeed = 15f;
    public float throwLoft = 4f;
    private const float ThrowMoveLockDuration = 0.3f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float cameraPitch = 0f;
    //private Throwable heldObject;
    private float throwLockTimer;

    //public bool IsJumping => !isGrounded;
    //public Throwable HeldObject => heldObject;

    [SerializeField] private Animator spriteAnimator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (spriteAnimator == null)
        {
            spriteAnimator = GetComponentInChildren<Animator>();
        }

        if (jumpStarParticles == null)
            jumpStarParticles = GetComponentInChildren<ParticleSystem>();

        if (jumpStarParticles != null)
        {
            var main = jumpStarParticles.main;
            main.playOnAwake = false;
            jumpStarParticles.Stop();
            jumpStarParticles.Clear();
        }
    }

    void Update()
    {
        //applianceInteraction.InputConsumed = false;
        bool controllerGrounded = controller.isGrounded;
        bool sphereGrounded = false;

        if (groundCheck != null && groundMask != 0)
            sphereGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        isGrounded = controllerGrounded || sphereGrounded;

        if (!isGrounded)
        {
            RaycastHit hit;
            float rayDistance = (controller.height / 2f) + 0.3f;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
                isGrounded = true;
        }

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Count down throw movement lock
        if (throwLockTimer > 0f)
            throwLockTimer -= Time.deltaTime;

        // Movement (suppressed briefly after throwing)
        float x = throwLockTimer > 0f ? 0f : Input.GetAxis("Horizontal");
        float z = throwLockTimer > 0f ? 0f : Input.GetAxis("Vertical");
        //Vector3 move = Vector3.right * x + Vector3.forward * z;
        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
        Vector3 move = camRight * x + camForward * z;

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) && isGrounded ? runSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump — blocked while holding an object so left-click throw doesn't also jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            PlayJumpEffect();
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKeyUp(KeyCode.Q))
        {
            GameObject activeItem = InventorySystem.Instance.TakeActiveItem();
            if (activeItem != null)
            {
                activeItem.transform.SetParent(null);
                activeItem.transform.position = transform.position + transform.forward * 0.6f;
                activeItem.SetActive(true);
                Throwable throwable = activeItem.GetComponent<Throwable>();
                if (throwable != null)
                {
                    throwable.Drop();
                }
                else
                {
                    Rigidbody rb2 = activeItem.GetComponent<Rigidbody>();
                    if (rb2 != null)
                    {
                        rb2.isKinematic = false;
                    }
                    Collider col2 = activeItem.GetComponent<Collider>();
                    if (col2 != null)
                    {
                        col2.enabled = true;
                    }
                }
            }
        }

        
        UpdateAnimation();
    }

    void PlayJumpEffect()
    {
        if (jumpStarParticles == null) return;

        jumpStarParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        jumpStarParticles.Clear();
        jumpStarParticles.Simulate(0, true, true);
        jumpStarParticles.Play();
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }

    void UpdateAnimation()
    {
        if (spriteAnimator == null)
        {
            return;
        }
        float rawX = Input.GetAxisRaw("Horizontal");
        float rawZ = Input.GetAxisRaw("Vertical");
        spriteAnimator.SetFloat("x", rawX);
        spriteAnimator.SetFloat("y", rawZ);
        spriteAnimator.SetBool("isMoving", rawX != 0f || rawZ != 0f);
        spriteAnimator.SetBool("isGrounded", isGrounded);

    }

    void TryPickupLooseItem()
    {
        if (InventorySystem.Instance.IsFull())
        {
            return;
        }

        Transform cam = playerCamera != null ? playerCamera : transform;
        Throwable found = null;

        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, pickupRadius))
        {
            found = hit.collider.GetComponentInParent<Throwable>();
        }

        if (found == null)
        {
            Collider[] nearby = Physics.OverlapSphere(transform.position, pickupRadius, ~0, QueryTriggerInteraction.Collide);
            foreach (var col in nearby)
            {
                Throwable t = col.GetComponentInParent<Throwable>();
                if (t != null)
                {
                    found = t;
                    break;
                }
            }
        }

        if (found != null)
        {
            found.PickUp(InventorySystem.Instance.holdPoint);
            InventorySystem.Instance.AddItem(found.gameObject);
        }
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E) && !applianceInteraction.InputConsumed)
        {
            TryPickupLooseItem();
        }

        //applianceInteraction.InputConsumed = false;
    }
}