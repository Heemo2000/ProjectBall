using UnityEngine;

public class LanePlayer : MonoBehaviour
{
    public float laneDistance = 3f;
    public float laneChangeSpeed = 10f;
    public float forwardSpeed = 5f;

    private int currentLane = 1;
    private Vector3 targetPosition;

    void Start()
    {
        UpdateTargetPosition();
    }

    void Update()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        Vector3 moveDirection = targetPosition - transform.position;
        moveDirection.y = 0;
        moveDirection.z = 0;

        transform.Translate(moveDirection * laneChangeSpeed * Time.deltaTime);
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

    private void UpdateTargetPosition()
    {
        targetPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
    }
}
