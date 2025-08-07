using Dreamteck.Forever;
using Game.Core;
using Game.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

public class LanePlayer : MonoBehaviour
{
    [Header("Lane Settings")]
    [SerializeField] float laneDistance = 3f;
    [SerializeField] float laneChangeSpeed = 10f;

    [Header("Movement")]
    [SerializeField] float baseSpeed = 5f;
    [SerializeField] float boostSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] float minJumpForce = 6f;
    [SerializeField] float maxJumpForce = 12f;
    [SerializeField] float jumpChargeRate = 5f;

    private float currentSpeed;
    private int currentLane = 2;

    private LaneRunner runner;
    private Rigidbody rb;

    private InputActions inputActions;
    private bool isHolding = false;
    private bool isChargingJump = false;
    private bool isGrounded = true;

    private float touchStartTime;
    private float tapThreshold = 0.1f;

    private float holdJumpForce;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        runner = GetComponent<LaneRunner>();
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
        rb.isKinematic = false;
    }

    void Update()
    {
        // Forward movement with gradual speed change, lane distance and lane movement
        float targetSpeed = isHolding ? boostSpeed : baseSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 5f * Time.deltaTime);
        runner.followSpeed = currentSpeed;
        runner.laneSwitchSpeed = laneChangeSpeed;
        runner.width = laneDistance;

        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // Jump charge
        if (isChargingJump && isGrounded)
        {
            holdJumpForce += jumpChargeRate * Time.deltaTime;
            holdJumpForce = Mathf.Clamp(holdJumpForce, minJumpForce, maxJumpForce);
        }

        //   Debug.Log("Current Speed: " + currentSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.TryGetComponent<Obstacle>(out Obstacle obstacle))
        {
            if(ServiceLocator.ForSceneOf(this).TryGetService<GameManager>(out GameManager gameManager))
            {
                gameManager.OnGameOver?.Invoke();
            }
        }
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
                Debug.Log("Quick tap jump" + minJumpForce);
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
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        Debug.Log("Jump with force: " + force);
    }

    public void MoveLeft()
    {
        if (currentLane > 1)
        {
            currentLane--;
            runner.lane = currentLane;
        }
    }

    public void MoveRight()
    {
        if (currentLane < 3)
        {
            currentLane++;
            runner.lane = currentLane;
        }
    }

    public void SetIsKinematic(bool isKinematic)
    {
        rb.isKinematic = isKinematic;
    }


}