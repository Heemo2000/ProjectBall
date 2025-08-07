using UnityEngine;
using UnityEngine.InputSystem;

public class LanePlayer : MonoBehaviour
{
    [Header("Lane Settings")]
    public float laneDistance = 3f;
    public float laneChangeSpeed = 10f;

    [Header("Movement")]
    public float baseSpeed = 5f;
    public float boostSpeed = 10f;
    public float jumpForce = 8f;

    private float currentSpeed;
    private int currentLane = 1;
    private Vector3 targetPosition;
    private Rigidbody rb;

    private InputActions inputActions;
    private bool isHolding = false;
    private bool isGrounded = true;

    private float touchStartTime;
    private float tapThreshold = 0.1f; // seconds

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new InputActions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Gameplay.TouchPress.started += ctx => OnTouchStart();
        inputActions.Gameplay.TouchPress.canceled += ctx => OnTouchEnd();
    }

    void OnDisable()
    {
        inputActions.Gameplay.TouchPress.started -= ctx => OnTouchStart();
        inputActions.Gameplay.TouchPress.canceled -= ctx => OnTouchEnd();
        inputActions.Disable();
    }

    void Start()
    {
        currentSpeed = baseSpeed;
        UpdateTargetPosition();
    }

    void Update()
    {
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        Vector3 moveDirection = targetPosition - transform.position;
        moveDirection.y = 0;
        moveDirection.z = 0;
        transform.Translate(moveDirection * laneChangeSpeed * Time.deltaTime);

        // Simple ground check
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f))
            isGrounded = true;
        else
            isGrounded = false;

        currentSpeed = isHolding ? boostSpeed : baseSpeed;
    }

    void OnTouchStart()
    {
        touchStartTime = Time.time;
        isHolding = true;
    }

    void OnTouchEnd()
    {
        float heldTime = Time.time - touchStartTime;

        if (heldTime < tapThreshold && isGrounded)
        {
            Jump(); // only jump if it was a quick tap
        }

        isHolding = false;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Debug.Log("Jump!");
    }

    public void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
            UpdateTargetPosition();
        }
    }

    public void MoveRight()
    {
        if (currentLane < 2)
        {
            currentLane++;
            UpdateTargetPosition();
        }
    }

    void UpdateTargetPosition()
    {
        targetPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
    }
}
