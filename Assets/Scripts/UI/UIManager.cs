using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace Game
{
    public class UIManager : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField] TextMeshProUGUI scoreText;
        public float score = 0;
        float Fscore = 0;
        [SerializeField] float scoreIncrementRate = 10f; // Points per second
        [SerializeField] TextMeshProUGUI scoreGameOver;

        [Header("Pause Game")]
        [SerializeField] GameObject Pausemenu;
        [SerializeField] GameObject GameOverScreen;

        void Start()
        {
            if(Pausemenu != null) Pausemenu.SetActive(false);
            if (GameOverScreen != null) GameOverScreen.SetActive(false);

        }


        void Update()
        {
            if (scoreText != null)        // score
            {
                float deltaTime = Time.deltaTime;
                 
                Fscore += deltaTime * scoreIncrementRate; // Increment score by  points per second
                score = Mathf.FloorToInt(Fscore);



                scoreText.text = "Score: " + score.ToString(); // Update the score text

               
                scoreGameOver.text = "Your Score\n" + score.ToString(); // Update the score text in Game Over screen
            }

        }

        public void PlayGame()
        {
            Time.timeScale = 1; // Ensure the game is running at normal speed
            SceneManager.LoadScene("GameScene"); // Load the game scene
        }

        public void pausegame()
        {
            Time.timeScale = 0; 
            Pausemenu.SetActive(true); 
        }

        public void resumeGame()
        {
            Time.timeScale = 1; 
            Pausemenu.SetActive(false); 
        }

        public void quitGame()
        {
            Application.Quit(); 
           
        }

        public void restartGame()
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
