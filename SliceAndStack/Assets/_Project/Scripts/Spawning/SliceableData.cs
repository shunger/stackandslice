using UnityEngine;

namespace SliceAndStack.Spawning
{
    [CreateAssetMenu(fileName = "NewSliceableData", menuName = "SliceAndStack/Sliceable Data")]
    public class SliceableData : ScriptableObject
    {
        [Header("Identity")]
        public string objectName;
        public string themeId = "default";

        [Header("Whole Object")]
        public Sprite wholeSprite;
        public float mass = 1f;
        public Vector2 colliderSize = new Vector2(1f, 1f);

        [Header("Sliced Halves")]
        public Sprite leftHalfSprite;
        public Sprite rightHalfSprite;
        public Vector2 leftHalfOffset = new Vector2(-0.25f, 0f);
        public Vector2 rightHalfOffset = new Vector2(0.25f, 0f);

        [Header("Scoring")]
        public int basePoints = 100;

        [Header("Visuals")]
        public Color sliceTrailColor = Color.white;
        public GameObject sliceParticlesPrefab;

        [Header("Audio")]
        public string sliceSoundId = "slice_default";
    }
}
