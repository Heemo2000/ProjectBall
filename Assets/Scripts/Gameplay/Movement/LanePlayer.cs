using Game;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class LanePlayer : MonoBehaviour
{
    [Header("Lane Settings")]
    public float laneDistance = 3f;
    public float laneChangeSpeed = 10f;

    [Header("Movement")]
    public float baseSpeed = 5f;
    public float boostSpeed = 10f;

    [Header("Jump Settings")]
    public float minJumpForce = 6f;
    public float maxJumpForce = 12f;
    public float jumpChargeRate = 5f;

    [Header("Movement Settings")]
    private float currentSpeed;
    private int currentLane = 1;
    private Vector3 targetPosition;
    private Rigidbody rb;

    private InputActions inputActions;
    private bool isHolding = false;
    private bool isChargingJump = false;
    private bool isGrounded = true;

    private float touchStartTime;
    private float tapThreshold = 0.1f;

    private float holdJumpForce;

    public UIManager uiManager; // Reference to UIManager for score updates
    public int[] speedUpgradeThresholds = { 300, 700, 1200, 1500 };
    private int currentThresholdIndex = 0;
    public float speedMultiplier = 2f;

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
        // Forward movement with gradual speed change
        float targetSpeed = isHolding ? boostSpeed : baseSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 5f * Time.deltaTime);
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Lane movement
        Vector3 moveDirection = targetPosition - transform.position;
        moveDirection.y = 0;
        moveDirection.z = 0;
        transform.Translate(moveDirection * laneChangeSpeed * Time.deltaTime);

        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // Jump charge
        if (isChargingJump && isGrounded)
        {
            holdJumpForce += jumpChargeRate * Time.deltaTime;
            holdJumpForce = Mathf.Clamp(holdJumpForce, minJumpForce, maxJumpForce);
        }

        Debug.Log("Current Speed: " + currentSpeed);
        CheckAndUpgradeSpeed();


    }

    void OnTouchStart()
    {
        touchStartTime = Time.time;
        isHolding = true;
        isChargingJump = true;
        holdJumpForce = minJumpForce;
    }

    void OnTouchEnd()
    {
        float heldTime = Time.time - touchStartTime;

        if (isGrounded)
        {
            if (heldTime < tapThreshold)
            {
                Jump(minJumpForce); // Quick tap jump
                Debug.Log("Quick tap jump"+ minJumpForce);
            }
            else
            {
                Jump(holdJumpForce); // Charged jump
                Debug.Log("Charged jump with force: " + holdJumpForce);
            }
        }

        isHolding = false;
        isChargingJump = false;
        holdJumpForce = 0f;
    }

    void Jump(float force)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Reset vertical velocity
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        Debug.Log("Jump with force: " + force);
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

    void CheckAndUpgradeSpeed()
    {
        if (currentThresholdIndex < speedUpgradeThresholds.Length &&
            uiManager.score >= speedUpgradeThresholds[currentThresholdIndex])
        {
            baseSpeed *= speedMultiplier;
            boostSpeed *= speedMultiplier;

            currentThresholdIndex++; // move to the next threshold so we don’t repeat this upgrade
        }
    }

}
