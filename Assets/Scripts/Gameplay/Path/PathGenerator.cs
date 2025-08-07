using UnityEngine;
using Dreamteck.Forever;
using System.Collections;
using Game.Core;

namespace Game.Gameplay
{
    public class PathGenerator : MonoBehaviour
    {
        [SerializeField] private float waitTimeAfterGenStart = 2.0f;
        [SerializeField] private LanePlayer player;
        private bool pathInitialized = false;

        private IEnumerator InitializePath()
        {
            if(pathInitialized)
            {
                yield break;
            }

            yield return new WaitUntil(() => LevelGenerator.instance == null || LevelGenerator.instance.ready == false);
            Debug.Log("Generating path");
            
            LevelGenerator.instance.StartGeneration();
            yield return new WaitForSeconds(waitTimeAfterGenStart);

            if(ServiceLocator.Global.TryGetService<GameManager>(out GameManager gameManager) &&
               ServiceLocator.Global.TryGetService<UIManager>(out UIManager uiManager))
            {
                gameManager.OnGameStarted.AddListener(Initialize);
                gameManager.OnScoreSet.AddListener(uiManager.SetScoreText);
                gameManager.OnGameStarted.AddListener(uiManager.ShowGameplayScreen);
                gameManager.OnGameOver.AddListener(uiManager.ShowGameOverScreen);

                gameManager.OnGameStarted?.Invoke();
            }
            pathInitialized = true;
        }

        private void Initialize()
        {
            player.SetIsKinematic(false);
            Debug.Log("Path Generator initialized");

        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(InitializePath());
        }
    }
}
