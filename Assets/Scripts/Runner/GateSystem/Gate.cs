using System;
using Runner.GeneratorSystem;
using Runner.ReagentSystem;
using UnityEngine;

namespace Runner.GateSystem
{
    public class Gate : MonoBehaviour
    {
        public event Action<Gate> OnActivate;

        protected virtual void OnCollision()
        {
            OnActivate?.Invoke(this);
        }

        public virtual void Activate()
        {
        }
    }
}