using UnityEngine;
using Dreamteck.Forever;
using System.Collections;

namespace Game.Gameplay
{
    public class PathGenerator : MonoBehaviour
    {
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
            pathInitialized = true;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
             StartCoroutine(InitializePath());
        }
    }
}
