using UnityEngine;

namespace Runner.PlayerComponent
{
    public class BoilerCollider : MonoBehaviour
    {
        public Boiler Boiler => _boiler;
        
        [SerializeField]
        private Boiler _boiler;
    }
}