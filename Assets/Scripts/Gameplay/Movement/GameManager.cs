using Game.Core;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] SwipeController swipeController;
    [SerializeField] LanePlayer player;

    [SerializeField] private float scoreIncreaseAmount = 2.0f;
    public UnityEvent OnGameStarted;
    public UnityEvent OnGameOver;
    public UnityEvent<float> OnScoreSet;

    private bool isGameStarted = false;
    private float currentScore = 0.0f;

    public float CurrentScore { get => currentScore;}

    private void StartGame()
    {
        isGameStarted = true;
    }

    private void EndGame()
    {
        isGameStarted = false;
    }

    private void Start()
    {
        ServiceLocator.ForSceneOf(this).Register<GameManager>(this);

        swipeController = GetComponent<SwipeController>();

        swipeController.OnSwipeLeft += player.MoveLeft;
        swipeController.OnSwipeRight += player.MoveRight;
        OnGameStarted.AddListener(StartGame);
        OnGameOver.AddListener(EndGame);
    }

    private void Update()
    {
        if(isGameStarted)
        {
            currentScore += scoreIncreaseAmount * Time.deltaTime;
            OnScoreSet?.Invoke(currentScore);
        }
    }
    private void OnDestroy()
    {
        swipeController.OnSwipeLeft -= player.MoveLeft;
        swipeController.OnSwipeRight -= player.MoveRight;
    }
}
