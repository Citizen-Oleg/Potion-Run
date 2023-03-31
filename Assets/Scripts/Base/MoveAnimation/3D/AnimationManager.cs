using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Base.MoveAnimation._3D
{
    public class AnimationManager : ITickable
    {
        private readonly List<AnimationInformation> _animationResourceItems = new List<AnimationInformation>();
        private readonly List<SettingsCurve> _settingsCurves;
        
        public AnimationManager(AnimationSettings animationSettings)
        {
            _settingsCurves = animationSettings.SettingsCurves;
        }

        public void ShowMoveObject(Transform item, Transform endTransformPosition, Vector3 offSet,  Vector3 endScale,
            Action callBack, bool useScale = false, bool useRotation = true, float travelTime = 0.35f, 
            CurveType curveType = CurveType.Duga, ActivePositionType activePositionType = ActivePositionType.None)
        {
            var information = new AnimationInformation
            {
                TravelTime = travelTime,
                AnimationCurve = GetCurveByType(curveType),
                Item = item,
                StartPosition = item.transform.position,
                UseRotation = useRotation,
                StartRotation = item.transform.rotation,
                CallBack = callBack,
                EndTransformPosition = endTransformPosition,
                Progress = 0,
                Offset = offSet,
                UseScale = useScale,
                ActivePositionType = activePositionType
            };

            if (useScale)
            {
                information.StartLocalScale = item.localScale;
                information.TargetLocalScale = endScale;
            }
            
            _animationResourceItems.Add(information);
        }
        
        public void Tick()
        {
            for (int i = 0; i < _animationResourceItems.Count; i++)
            {
                var positionYcurve = Vector3.zero;
                var information = _animationResourceItems[i];
                
                positionYcurve = information.AnimationCurve.Evaluate(information.Progress) * Vector3.up;
                
                information.Progress += Time.deltaTime / information.TravelTime;

                var endTransform = information.EndTransformPosition;
                var endPosition = endTransform.TransformPoint(information.Offset);
                var positionItem = Vector3.Lerp(information.StartPosition, endPosition, information.Progress)
                                   + positionYcurve;

                SetPosition(information.Item.transform, positionItem, information.ActivePositionType);

                if (information.UseRotation)
                {
                    var rotationItem = Quaternion.Lerp(information.StartRotation, endTransform.rotation,
                        information.Progress);
                    information.Item.transform.rotation = rotationItem;
                }

                if (information.UseScale)
                {
                    information.Item.localScale = Vector3.Lerp(information.StartLocalScale,
                        information.TargetLocalScale, information.Progress);
                }

                _animationResourceItems[i] = information;
                if (information.Progress >= 1)
                {
                    information.CallBack?.Invoke();
                    _animationResourceItems.RemoveAt(i);
                    i--;
                }
            }
        }

        private void SetPosition(Transform transform, Vector3 position, ActivePositionType activePositionType)
        {
            switch (activePositionType)
            {
                case ActivePositionType.None:
                    transform.position = position;
                    break;
                case ActivePositionType.X:
                    var xPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    transform.position = xPosition;
                    break;
                case ActivePositionType.Y:
                    var yPosition = new Vector3(transform.position.x, position.y, transform.position.z);
                    transform.position = yPosition;
                    break;
                case ActivePositionType.Z:
                    var zPosition = new Vector3(transform.position.x, transform.position.y, position.z);
                    transform.position = zPosition;
                    break;
            }
        }

        private AnimationCurve GetCurveByType(CurveType curveType)
        {
            return _settingsCurves.FirstOrDefault(curve => curve.CurveType == curveType).AnimationCurve;
        }
    }
}