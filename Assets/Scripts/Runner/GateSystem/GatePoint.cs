using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runner.GateSystem
{
    public class GatePoint : MonoBehaviour
    {
        [SerializeField]
        private List<Gate> _gates = new List<Gate>();

        private bool _isBlock;
        
        private void Awake()
        {
            foreach (var gate in _gates)
            {
                gate.OnActivate += BlockGates;
            }
        }

        private void OnEnable()
        {
            _isBlock = false;
        } 

        private void BlockGates(Gate gate)
        {
            if (_isBlock)
            {
                return;
            }

            _isBlock = true;
            gate.Activate();
        }

        private void OnDestroy()
        {
            foreach (var gate in _gates)
            {
                gate.OnActivate -= BlockGates;
            }
        }
    }
}