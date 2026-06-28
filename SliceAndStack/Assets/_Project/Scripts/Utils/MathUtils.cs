using UnityEngine;

namespace SliceAndStack.Utils
{
    public static class MathUtils
    {
        public static Vector2 CalculateCenterOfMass(Rigidbody2D[] bodies)
        {
            if (bodies == null || bodies.Length == 0)
                return Vector2.zero;

            float totalMass = 0f;
            Vector2 weightedSum = Vector2.zero;

            foreach (var body in bodies)
            {
                if (body == null) continue;
                float mass = body.mass;
                totalMass += mass;
                weightedSum += (Vector2)body.worldCenterOfMass * mass;
            }

            return totalMass > 0f ? weightedSum / totalMass : Vector2.zero;
        }

        public static float NormalizeToRange(float value, float min, float max)
        {
            if (max <= min) return 0f;
            return Mathf.Clamp01((value - min) / (max - min));
        }

        public static float InverseLerp01(float a, float b, float value)
        {
            if (Mathf.Approximately(a, b)) return 0f;
            return Mathf.Clamp01((value - a) / (b - a));
        }

        public static float SmoothDamp01(float current, float target, ref float velocity, float smoothTime)
        {
            return Mathf.SmoothDamp(current, target, ref velocity, smoothTime);
        }

        public static float ScreenPercentToWorldY(float percent, Camera camera)
        {
            var bottom = camera.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f));
            var top = camera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0f));
            return Mathf.Lerp(bottom.y, top.y, percent);
        }

        public static float WorldYToScreenPercent(float worldY, Camera camera)
        {
            var bottom = camera.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f));
            var top = camera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0f));
            return Mathf.InverseLerp(bottom.y, top.y, worldY);
        }
    }
}
