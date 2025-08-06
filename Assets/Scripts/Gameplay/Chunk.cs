using UnityEngine;
using Dreamteck.Splines;

namespace Game.Gameplay
{
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private Transform endPoint;
        
        private SplineComputer splineComputer;
        private SplineMesh splineMesh;
        
        public Transform EndPoint { get => endPoint; }

        public void SetType(Spline.Type type)
        {
            splineComputer.type = type;
        }

        public void SetSpace(SplineComputer.Space space)
        {
            splineComputer.space = space;
        }

        public void Initialize(SplinePoint[] points, 
                               Mesh[] meshes, 
                               int meshCount)
        {
            splineComputer.SetPoints(points, SplineComputer.Space.World);
            splineComputer.Rebuild();
            while (splineMesh.GetChannelCount() > 0)
            {
                splineMesh.RemoveChannel(splineMesh.GetChannelCount() - 1);
            }
            
            for(int i = 0; i < meshes.Length; i++)
            {
                var mesh = meshes[i];
                var channel = splineMesh.AddChannel(mesh, mesh.name + i.ToString());
                channel.count = meshCount;
            }

            var sample = splineComputer.Evaluate(1.0f);
            endPoint.position = sample.position;
            endPoint.forward = sample.forward;
            splineMesh.Bake(false, false);
            
        }

        private void Awake()
        {
            splineComputer = GetComponent<SplineComputer>();
            splineMesh = GetComponent<SplineMesh>();
        }


    }
}
