using UnityEngine;
using Dreamteck.Forever;
using System.Collections;
using Game.Core;

namespace Game.Gameplay
{
    public class PathGenerator : MonoBehaviour
    {
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
            player.SetIsKinematic(true);
            //player.SetIsTrigger(false);
            LevelGenerator.instance.StartGeneration();
            yield return new WaitUntil(() => LevelGenerator.instance.generationProgress < 1.0f);    
            
            GameManager gameManager = null;
            UIManager uiManager = null;

            ServiceLocator.ForSceneOf(this).TryGetService<GameManager>(out gameManager);
            ServiceLocator.Global.TryGetService<UIManager>(out uiManager);
            Debug.Log("Is GameManager null ? " + (gameManager == null));
            Debug.Log("Is UIManager null ? " + (uiManager == null));

            gameManager.OnGameStarted.AddListener(Initialize);
            gameManager.OnScoreSet.AddListener(uiManager.SetScoreText);
            gameManager.OnGameStarted.AddListener(uiManager.ShowGameplayScreen);
            gameManager.OnGameOver.AddListener(uiManager.ShowGameOverScreen);

            gameManager.OnGameStarted?.Invoke();

            
            pathInitialized = true;
        }

        private void Initialize()
        {
            player.SetIsKinematic(false);
            //player.SetIsTrigger(true);
            Debug.Log("Path Generator initialized");
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(InitializePath());
        }
    }
}
