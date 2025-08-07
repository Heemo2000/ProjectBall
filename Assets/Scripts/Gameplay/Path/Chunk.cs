using UnityEngine;
using Dreamteck.Forever;
using Dreamteck.Splines;
using Game.Core;

namespace Game.Gameplay
{
    public class Chunk : MonoBehaviour
    {
        
        [Min(1.0f)]
        [SerializeField] private float width = 5.0f;
        [Min(0.1f)]
        [SerializeField] private float height = 5.0f;
        private LevelSegment segment;

        private void GenerateObstacles()
        {
            if(!ServiceLocator.ForSceneOf(this).TryGetService<ObstaclesManager>(out ObstaclesManager obstaclesManager))
            {
                return;
            }

            var bounds = segment.GetBounds();

            double randomPercent = Random.value;
            SplineSample sample = new SplineSample();
            segment.Evaluate(randomPercent, ref sample);

            int randomObstaclesCount = Random.Range(1, 4);

            Vector3 origin = sample.position + Vector3.up * height / 2.0f;
            int direction = 0;
            while(randomObstaclesCount > 0)
            {
                //Get direction between -1 and 1.
                direction = Random.Range(-1, 2);

                //Calculate spawn position based on that direction.
                Vector3 spawnPosition = origin + 
                                        direction * sample.right *  width / 4.0f;

                if(obstaclesManager.IsObstacleThere(spawnPosition))
                {
                    continue;
                }

                Quaternion spawnRotation = Quaternion.LookRotation(sample.forward, sample.up);
                obstaclesManager.Spawn(spawnPosition, spawnRotation);
                randomObstaclesCount--;
                
            }
        }

        private void Awake()
        {
            segment = GetComponent<LevelSegment>();
        }

        private void OnEnable()
        {
            segment.onExtruded += GenerateObstacles;
        }

        private void OnDisable()
        {
            segment.onExtruded -= GenerateObstacles;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 left = Vector3.zero;
            Vector3 right = Vector3.zero;

            Vector3 up = Vector3.zero;
            Vector3 down = Vector3.zero;

            if(!Application.isPlaying)
            {
                left = transform.position - transform.right * width / 2.0f;
                right = transform.position + transform.right * width / 2.0f;
                up = transform.position + transform.up * height / 2.0f;
                down = transform.position - transform.up * height / 2.0f;
            }
            else
            {
                SplineSample sample = new SplineSample();
                segment.Evaluate(0.5f, ref sample);
                left = sample.position - sample.right * width / 2.0f;
                right = sample.position + sample.right * width / 2.0f;
                up = sample.position + sample.up * height / 2.0f;
                down = sample.position - sample.up * height / 2.0f;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(left, right);
            Gizmos.DrawLine(up, down);
        }
    }
}
