using UnityEngine;

namespace SliceAndStack.Difficulty
{
    [CreateAssetMenu(fileName = "DifficultyConfig", menuName = "SliceAndStack/Config/Difficulty Config")]
    public class DifficultyConfig : ScriptableObject
    {
        [Header("Score-based progression (X = score, Y = value)")]

        [Tooltip("Spawn interval in seconds (higher = slower spawns)")]
        public AnimationCurve spawnInterval = new AnimationCurve(
            new Keyframe(0, 3.0f),
            new Keyframe(500, 2.2f),
            new Keyframe(1500, 1.5f),
            new Keyframe(3000, 1.0f)
        );

        [Tooltip("Fall speed in units/second")]
        public AnimationCurve fallSpeed = new AnimationCurve(
            new Keyframe(0, 2.0f),
            new Keyframe(500, 3.0f),
            new Keyframe(1500, 4.5f),
            new Keyframe(3000, 6.0f)
        );

        [Tooltip("Slice zone size as fraction of screen height")]
        public AnimationCurve sliceZoneSize = new AnimationCurve(
            new Keyframe(0, 0.25f),
            new Keyframe(500, 0.20f),
            new Keyframe(1500, 0.15f),
            new Keyframe(3000, 0.10f)
        );

        [Tooltip("Wind strength (0 = none)")]
        public AnimationCurve windStrength = new AnimationCurve(
            new Keyframe(0, 0f),
            new Keyframe(1000, 0f),
            new Keyframe(1500, 0.5f),
            new Keyframe(3000, 1.5f)
        );

        [Header("Wind Settings")]
        public float windChangeInterval = 3f;
        public float windSmoothTime = 1f;
    }
}
