using System.Collections.Generic;
using UnityEngine;

namespace SliceAndStack.Cosmetics
{
    [CreateAssetMenu(fileName = "NewTheme", menuName = "SliceAndStack/Cosmetics/Theme Data")]
    public class ThemeData : ScriptableObject
    {
        [Header("Identity")]
        public string themeId;
        public string displayName;
        public Sprite icon;

        [Header("Colors")]
        public Color backgroundColor = new Color(0.1f, 0.1f, 0.2f);
        public Color sliceZoneColor = new Color(1f, 1f, 1f, 0.15f);
        public Color towerBaseColor = Color.gray;
        public Color uiAccentColor = Color.cyan;

        [Header("Sliceable Objects")]
        public List<Spawning.SliceableData> sliceables;

        [Header("Visual Effects")]
        public Color sliceParticleColor = Color.white;
        public Color landingParticleColor = Color.gray;
        public Sprite backgroundSprite;

        [Header("Audio")]
        public string musicTrackId;
    }
}
