using UnityEngine;

namespace Base.Tools
{
    public class UIUtility
    {
        public static Vector2 WorldToCanvasAnchoredPosition(Camera camera, RectTransform parent, Vector3 attachPoint)
        {
            var worldPoint = attachPoint;
            var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, null,
                out var localPoint);

            return localPoint;
        }

        public static Vector2 WorldToCanvasPosition(Camera camera, Vector3 attachPoint)
        {
            var worldPoint = attachPoint;
            var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPoint);
            return screenPoint;
        }
    }
}