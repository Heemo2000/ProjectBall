using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Game.Core;

namespace Game
{
    public class UIManager : MonoBehaviour
    {
        [Header("All Screens")]
        [SerializeField] private GameObject mainMenuScreen;
        [SerializeField] private GameObject pauseMenuScreen;
        [SerializeField] private GameObject gameplayScreen;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] GameObject gameOverScreen;

        [Header("Game Scene")]
        [SerializeField] private string gameSceneName = "SampleScene";

        [Header("Score")]
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] TextMeshProUGUI scoreGameOver;

        public void SetScoreText(float score)
        {
            scoreText.text = Mathf.FloorToInt(score).ToString();
            scoreGameOver.text = Mathf.FloorToInt(score).ToString();
        }

        void Start()
        {
            ShowMainMenuScreen();
            DontDestroyOnLoad(this.gameObject);
            ServiceLocator.For(this).Register<UIManager>(this);
        }

        public void PlayGame()
        {
            Time.timeScale = 1; // Ensure the game is running at normal speed
            SceneManager.LoadScene(gameSceneName); // Load the game scene
        }

        public void ShowMainMenuScreen()
        {
            mainMenuScreen.SetActive(true);

            loadingScreen.SetActive(false);
            gameplayScreen.SetActive(false);
            pauseMenuScreen.SetActive(false);
            gameOverScreen.SetActive(false);
        }
        public void ShowLoadingScreen()
        
        {
            loadingScreen.SetActive(true);
            mainMenuScreen.SetActive(false);
            gameplayScreen.SetActive(false);
            pauseMenuScreen.SetActive(false);
            gameOverScreen.SetActive(false);
        }
        public void ShowPauseMenu()
        
        {
            pauseMenuScreen.SetActive(true);
            mainMenuScreen.SetActive(false);
            loadingScreen.SetActive(false);
            gameplayScreen.SetActive(false);
            gameOverScreen.SetActive(false);
        }


        public void ShowGameplayScreen()
        {
            Debug.Log("Showing gameplay screen");
            gameplayScreen.SetActive(true);
            
            mainMenuScreen.SetActive(false);
            loadingScreen.SetActive(false);
            pauseMenuScreen.SetActive(false);
            gameOverScreen.SetActive(false);
        }

        public void ShowGameOverScreen()
        {
            gameOverScreen.SetActive(true);
            gameplayScreen.SetActive(false);
            mainMenuScreen.SetActive(false);
            loadingScreen.SetActive(false);
            pauseMenuScreen.SetActive(false);
        }

        public void QuitGame()
        {
            Debug.Log("Quiting application");
            Application.Quit();    
        }

        public void RestartGame()
        {
           Time.timeScale = 1; 
           SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart the current scene
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1; 
            SceneManager.LoadScene("MainMenu"); // Load the main menu scene
        }
    }
}
