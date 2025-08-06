using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.ObjectPoolHandling;
using Dreamteck.Splines;

namespace Game.Gameplay
{
    public class PathGenerator : MonoBehaviour
    {
        [Header("Chunk Settings: ")]
        [SerializeField] private Chunk chunkPrefab;
        [SerializeField] private SpawnData[] chunksSpawnData;

        [Header("Straight Path Settings: ")]
        [Min(5)]
        [SerializeField] private int straightPathCount = 5;

        [Header("Other Settings:")]
        [Min(5)]
        [SerializeField] private int otherPathCount = 5;

        private Dictionary<int, ObjectPool<Chunk>> splinesDict;
        private bool isGenerated;
        private Vector3 lastSpawnPosition = Vector3.zero;

        private void SpawnStraightPaths()
        {
            var shapeCurve = chunksSpawnData[0].shapeCurve;
            var meshCount = chunksSpawnData[0].meshCount;
            var meshes = chunksSpawnData[0].meshes;
            lastSpawnPosition = transform.position;

            var length = chunksSpawnData[0].length;
            for(int i = 0; i < straightPathCount; i++)
            {
                var chunk = splinesDict[chunksSpawnData[0].GetHashCode()].Get();
                chunk.transform.position = lastSpawnPosition;
                var splinePoints = GetSplinePoints(lastSpawnPosition, shapeCurve, meshCount, length);
                chunk.Initialize(splinePoints, meshes, meshCount);
                lastSpawnPosition = chunk.EndPoint.position;
            }
        }

        private void SpawnRandomPaths()
        {
            int randomIndex = Random.Range(1, chunksSpawnData.Length);
            var randomSpawnData = chunksSpawnData[randomIndex];
            var shapeCurve = randomSpawnData.shapeCurve;
            var meshCount = randomSpawnData.meshCount;
            var meshes = randomSpawnData.meshes;
            
            var length = randomSpawnData.length;

            for (int i = 0; i < otherPathCount; i++)
            {
                var chunk = splinesDict[randomSpawnData.GetHashCode()].Get();
                chunk.SetType(Spline.Type.CatmullRom);
                chunk.SetSpace(SplineComputer.Space.World);

                chunk.transform.position = lastSpawnPosition;
                var splinePoints = GetSplinePoints(lastSpawnPosition, shapeCurve, meshCount, length);
                chunk.Initialize(splinePoints, meshes, meshCount);
                lastSpawnPosition = chunk.EndPoint.position;
            }
        }

        private SplinePoint[] GetSplinePoints(Vector3 origin, AnimationCurve shapeCurve, int pointsCount, float length)
        {
            var splinePoints = new SplinePoint[pointsCount];
            float percent = 0.0f;
            float increment = 1.0f / (float)pointsCount;
            for(int i = 0; i <  pointsCount; i++)
            {
                var positionZ = percent * length;
                var positionX = shapeCurve.Evaluate(percent) * length;
                var calculatedPos = origin + new Vector3(positionX, 0.0f, positionZ);
                
                Vector3 direction = (i - 1 >= 0) ? (calculatedPos - splinePoints[i - 1].position).normalized: calculatedPos.normalized;

                var point = new SplinePoint();
                point.type = SplinePoint.Type.SmoothFree;
                point.SetNormalX(0.0f);
                point.SetNormalY(1.0f);
                point.SetNormalZ(0.0f);

                point.SetPosition(calculatedPos);
                point.SetTangentPosition(direction);

                splinePoints[i] = point;
                percent += increment;
            }
            return splinePoints;
        }

        private void Awake()
        {
            splinesDict = new Dictionary<int, ObjectPool<Chunk>>();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if(!isGenerated)
            {
                foreach(var data in chunksSpawnData)
                {
                    var hashCode = data.GetHashCode();
                    var shapeCurve = data.shapeCurve;
                    var count = data.chunkCount;

                    var pool = new ObjectPool<Chunk>(() => CreateChunk(chunkPrefab),
                                                           OnChunkGet,
                                                           OnChunkReturnToPool,
                                                           OnChunkDestroy,
                                                           count);
                    
                    splinesDict.Add(hashCode, pool);
                }

                SpawnStraightPaths();
                SpawnRandomPaths();

                isGenerated = true;
            }
        }
        private Chunk CreateChunk(Chunk chunk)
        {
            var instance = Instantiate(chunk, Vector3.zero, Quaternion.identity);
            instance.transform.parent = transform;
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void OnChunkGet(Chunk chunk)
        {
            chunk.gameObject.SetActive(true);
        }

        private void OnChunkReturnToPool(Chunk chunk)
        {
            chunk.gameObject.SetActive(false);
        }

        private void OnChunkDestroy(Chunk chunk)
        {
            Destroy(chunk.gameObject);
        }
        
    }
}
