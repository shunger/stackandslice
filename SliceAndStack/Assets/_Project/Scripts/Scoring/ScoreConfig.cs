using UnityEngine;

namespace SliceAndStack.Scoring
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "SliceAndStack/Config/Score Config")]
    public class ScoreConfig : ScriptableObject
    {
        [Header("Quality Multipliers")]
        public int perfectMultiplier = 3;
        public int greatMultiplier = 2;
        public int goodMultiplier = 1;

        [Header("Combo")]
        public float comboMultiplierPerStep = 0.1f;
        public int maxComboMultiplier = 10;

        [Header("Height Bonus")]
        public float heightMultiplierBase = 1.0f;
        public float heightMultiplierPerMeter = 0.05f;

        [Header("Streak")]
        public int onFireThreshold = 5;
        public int rainbowThreshold = 10;
        public int streakBonusPoints = 50;
        public int streakBonusPerLevel = 25;
    }
}
