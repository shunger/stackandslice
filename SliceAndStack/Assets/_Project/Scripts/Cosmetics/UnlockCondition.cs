using UnityEngine;
using SliceAndStack.Persistence;

namespace SliceAndStack.Cosmetics
{
    [CreateAssetMenu(fileName = "NewUnlockCondition", menuName = "SliceAndStack/Cosmetics/Unlock Condition")]
    public class UnlockCondition : ScriptableObject
    {
        public enum ConditionType
        {
            ScoreReached,
            GamesPlayed,
            TotalSlices,
            TotalPerfects,
            TowerHeightReached
        }

        public string themeId;
        public ConditionType type;
        public int requiredValue;
        public string description;

        public bool IsMet(SaveData data)
        {
            return type switch
            {
                ConditionType.ScoreReached => data.highScore >= requiredValue,
                ConditionType.GamesPlayed => data.totalGamesPlayed >= requiredValue,
                ConditionType.TotalSlices => data.totalSlices >= requiredValue,
                ConditionType.TotalPerfects => data.totalPerfects >= requiredValue,
                ConditionType.TowerHeightReached => data.highestTower >= requiredValue,
                _ => false
            };
        }

        public float GetProgress(SaveData data)
        {
            if (requiredValue <= 0) return 1f;

            float current = type switch
            {
                ConditionType.ScoreReached => data.highScore,
                ConditionType.GamesPlayed => data.totalGamesPlayed,
                ConditionType.TotalSlices => data.totalSlices,
                ConditionType.TotalPerfects => data.totalPerfects,
                ConditionType.TowerHeightReached => data.highestTower,
                _ => 0f
            };

            return Mathf.Clamp01(current / requiredValue);
        }
    }
}
