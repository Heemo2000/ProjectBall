using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SwipeController swipeController;
    public LanePlayer player;

    private void Start()
    {
        swipeController = GetComponent<SwipeController>();

        swipeController.OnSwipeLeft += player.MoveLeft;
        swipeController.OnSwipeRight += player.MoveRight;
    }
}
