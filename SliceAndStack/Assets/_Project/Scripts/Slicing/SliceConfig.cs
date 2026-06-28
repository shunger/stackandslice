using UnityEngine;

namespace SliceAndStack.Slicing
{
    [CreateAssetMenu(fileName = "SliceConfig", menuName = "SliceAndStack/Config/Slice Config")]
    public class SliceConfig : ScriptableObject
    {
        [Header("Slice Zone Visuals")]
        public Color sliceZoneColor = new Color(1f, 1f, 1f, 0.15f);
        public Color sliceZonePerfectColor = new Color(1f, 0.84f, 0f, 0.3f);

        [Header("Quality Thresholds (normalized 0-1 within zone)")]
        [Range(0f, 1f)] public float perfectThreshold = 0.9f;
        [Range(0f, 1f)] public float greatThreshold = 0.7f;

        [Header("Slice Force")]
        public float sliceSeparationForce = 2.0f;
        public float sliceUpwardForce = 1.5f;
        public float sliceTorque = 50f;

        [Header("Sliced Half Settings")]
        public float halfGravityDelay = 0.15f;
        public float halfFadeTime = 0.3f;
    }
}
