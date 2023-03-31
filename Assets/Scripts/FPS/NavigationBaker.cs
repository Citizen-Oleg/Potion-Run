using UnityEngine;
using UnityEngine.AI;

namespace FPS
{
    public class NavigationBaker : MonoBehaviour
    {
        public NavMeshSurface[] surfaces;

        public void Bake()
        {
            for (int i = 0; i < surfaces.Length; i++) 
            {
                surfaces[i].BuildNavMesh();    
            }    
        }
    }
}