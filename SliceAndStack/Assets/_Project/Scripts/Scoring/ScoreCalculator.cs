using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Scoring
{
    public class ScoreCalculator
    {
        private readonly ScoreConfig _config;

        public ScoreCalculator(ScoreConfig config)
        {
            _config = config;
        }

        public int Calculate(int basePoints, SliceQuality quality, float comboMultiplier,
            float towerHeight, int streakBonus)
        {
            int qualityMult = GetQualityMultiplier(quality);
            float heightMult = GetHeightMultiplier(towerHeight);

            float raw = basePoints * qualityMult * comboMultiplier * heightMult + streakBonus;
            return Mathf.RoundToInt(raw);
        }

        public int GetQualityMultiplier(SliceQuality quality)
        {
            return quality switch
            {
                SliceQuality.Perfect => _config.perfectMultiplier,
                SliceQuality.Great => _config.greatMultiplier,
                SliceQuality.Good => _config.goodMultiplier,
                _ => 0
            };
        }

        public float GetHeightMultiplier(float towerHeight)
        {
            return _config.heightMultiplierBase + towerHeight * _config.heightMultiplierPerMeter;
        }
    }
}
