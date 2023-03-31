using System;
using UnityEngine;

namespace Base.MoveAnimation._3D
{
    public struct AnimationInformation
    {
        public float TravelTime;
        public AnimationCurve AnimationCurve;
        public Transform Item;
        public Vector3 StartPosition;
        public bool UseRotation;
        public Quaternion StartRotation;
        public Transform EndTransformPosition;
        public float Progress;
        public Action CallBack;
        public Vector3 Offset;

        public bool UseScale;
        public Vector3 StartLocalScale;
        public Vector3 TargetLocalScale;

        public ActivePositionType ActivePositionType;
    }
}