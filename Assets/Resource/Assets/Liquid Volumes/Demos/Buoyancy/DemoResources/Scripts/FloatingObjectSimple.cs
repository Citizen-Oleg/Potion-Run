using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiquidVolumeFX {

    public class FloatingObjectSimple : MonoBehaviour
    {
        public LiquidVolume liquidVolume;

        void Update() {

            if (liquidVolume != null)
            {
                liquidVolume.MoveToLiquidSurface(transform, BuoyancyEffect.Simple);
            }
        }
    }
}
