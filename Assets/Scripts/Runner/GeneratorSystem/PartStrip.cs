using System.Collections.Generic;
using Runner.GateSystem;
using Runner.ReagentSystem;
using UnityEngine;

namespace Runner.GeneratorSystem
{
    public class PartStrip : MonoBehaviour
    {
        public int ID;
        public ReagentType RewardReagent;
        public Transform ForwardPoint;
        public Transform RightPoint;
    }
}