using UnityEngine;
using Game.ObjectPoolHandling;
using Game.Core;

namespace Game.Gameplay
{
    public class ObstaclesManager : MonoBehaviour
    {
        [SerializeField] private Obstacle[] obstaclePrefabs;
        [SerializeField] private LayerMask obstacleLayerMask;
        [Min(0.01f)]
        [SerializeField] private float obstacleCheckRadius = 0.3f;
        [Min(5)]
        [SerializeField] private int obstacleCountLimit = 20;
        private ObjectPool<Obstacle> obstaclePool;

        public bool IsObstacleThere(Vector3 position)
        {
            return Physics.CheckSphere(position, obstacleCheckRadius, obstacleLayerMask.value);
        }

        public Obstacle Spawn(Vector3 position, Quaternion rotation)
        {
            Debug.Log("Getting obstacle");
            var instance = obstaclePool.Get();
            instance.transform.position = position;
            instance.transform.rotation = rotation;

            return instance;
        }

        public void Unspawn(Obstacle obstacle)
        {
            Debug.Log("Unspawning obstacle");
            obstaclePool.ReturnToPool(obstacle);
        }

        private Obstacle CreateObstacle()
        {
            int randomIndex = Random.Range(0, obstaclePrefabs.Length);
            var instance = Instantiate(obstaclePrefabs[randomIndex], Vector3.zero, Quaternion.identity);
            instance.transform.parent = transform;
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void OnGetObstacle(Obstacle obstacle)
        {
            obstacle.gameObject.SetActive(true);
        }

        private void OnReturnObstacle(Obstacle obstacle)
        {
            obstacle.gameObject.SetActive(false);
        }

        private void OnDestroyObstacle(Obstacle obstacle)
        {
            Destroy(obstacle.gameObject);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ServiceLocator.ForSceneOf(this).Register<ObstaclesManager>(this);

            if (obstaclePool == null)
            {
                obstaclePool = new ObjectPool<Obstacle>(CreateObstacle,
                                                        OnGetObstacle,
                                                        OnReturnObstacle,
                                                        OnDestroyObstacle,
                                                        obstacleCountLimit);           
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, obstacleCheckRadius);
        }

    }
}
