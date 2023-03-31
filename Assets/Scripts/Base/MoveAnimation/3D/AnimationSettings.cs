using System;
using System.Collections.Generic;
using UnityEngine;

namespace Base.MoveAnimation._3D
{
    [Serializable]
    public class AnimationSettings
    {
        public float TravelTime => _travelTime;
        public List<SettingsCurve> SettingsCurves => _settingsCurves;

        [SerializeField]
        private float _travelTime;
        [SerializeField]
        private List<SettingsCurve> _settingsCurves = new List<SettingsCurve>();
    }

    [Serializable]
    public class SettingsCurve
    {
        public CurveType CurveType;
        public AnimationCurve AnimationCurve;
    }

    public enum CurveType
    {
        Line = 0,
        Duga = 1
    }
}