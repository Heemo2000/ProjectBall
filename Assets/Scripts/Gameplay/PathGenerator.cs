using System.Collections.Generic;
using UnityEngine;
using Game.ObjectPoolHandling;
using Dreamteck.Splines;

namespace Game.Gameplay
{
    public class PathGenerator : MonoBehaviour
    {
        [SerializeField] private ChunksData[] chunksRelatedData;

        private Dictionary<ChunkShape, List<ObjectPool<Chunk>>> splinesDict;
        private bool isGenerated;


        private void Awake()
        {
            splinesDict = new Dictionary<ChunkShape, List<ObjectPool<Chunk>>>();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if(!isGenerated)
            {
                foreach(var data in chunksRelatedData)
                {
                    var shape = data.shape;
                    var splines = data.chunks;
                    var count = data.maxCount;

                    var list = new List<ObjectPool<Chunk>>();
                    foreach(var spline in splines)
                    {
                        var pool = new ObjectPool<Chunk>(() => CreateSpline(spline),
                                                              OnSplineGet,
                                                              OnSplineReturnToPool,
                                                              OnSplineDestroy,
                                                              count);

                        list.Add(pool);
                    }

                    splinesDict.Add(shape, list);
                }

                Vector3 spawnPosition = transform.position;
                Vector3 forwardDirection = transform.forward;

                foreach(var data in chunksRelatedData)
                {
                    var shape = data.shape;
                    for(var i = 0; i < 5; i++)
                    {
                        var list = splinesDict[shape];
                        var spline = list[Random.Range(0, list.Count)].Get();
                        var mesh = spline.GetComponent<SplineMesh>();
                        spline.transform.position = spawnPosition;
                        spline.transform.forward = forwardDirection;

                        spawnPosition = spline.EndPoint.position;
                        forwardDirection = spline.EndPoint.forward;
                        mesh.Bake(false, false);
                        
                    }
                }
                isGenerated = true;
            }
        }
        private Chunk CreateSpline(Chunk chunk)
        {
            var instance = Instantiate(chunk, Vector3.zero, Quaternion.identity);
            instance.transform.parent = transform;
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void OnSplineGet(Chunk chunk)
        {
            chunk.gameObject.SetActive(true);
        }

        private void OnSplineReturnToPool(Chunk chunk)
        {
            chunk.gameObject.SetActive(false);
        }

        private void OnSplineDestroy(Chunk chunk)
        {
            Destroy(chunk.gameObject);
        }
        
    }
}
