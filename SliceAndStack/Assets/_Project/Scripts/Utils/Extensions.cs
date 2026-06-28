using UnityEngine;

namespace SliceAndStack.Utils
{
    public static class Extensions
    {
        public static Vector2 WorldToViewport(this Camera camera, Vector3 worldPos)
        {
            return camera.WorldToViewportPoint(worldPos);
        }

        public static Vector3 ViewportToWorld(this Camera camera, Vector2 viewportPos, float z = 0f)
        {
            return camera.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, z));
        }

        public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }

        public static bool IsOnScreen(this Vector3 worldPos, Camera camera, float margin = 0f)
        {
            var vp = camera.WorldToViewportPoint(worldPos);
            return vp.x >= -margin && vp.x <= 1f + margin &&
                   vp.y >= -margin && vp.y <= 1f + margin;
        }

        public static void SetAlpha(this SpriteRenderer renderer, float alpha)
        {
            var color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }

        public static void SetAlpha(this UnityEngine.UI.Image image, float alpha)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();
            return comp != null ? comp : go.AddComponent<T>();
        }

        public static void DestroyAllChildren(this Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
                Object.Destroy(t.GetChild(i).gameObject);
        }

        public static string FormatScore(this int score)
        {
            return score.ToString("N0");
        }

        public static string FormatHeight(this float height)
        {
            return $"{height:F1}m";
        }
    }
}
