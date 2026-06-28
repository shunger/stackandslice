using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Scoring
{
    public class ComboTracker : MonoBehaviour
    {
        [SerializeField] private ScoreConfig _config;

        public int CurrentCombo { get; private set; }
        public int PerfectStreak { get; private set; }
        public bool IsOnFire => PerfectStreak >= _config.onFireThreshold;
        public bool IsRainbow => PerfectStreak >= _config.rainbowThreshold;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public void RegisterHit(SliceQuality quality)
        {
            CurrentCombo++;

            if (quality == SliceQuality.Perfect)
            {
                PerfectStreak++;
            }
            else
            {
                PerfectStreak = 0;
            }

            EventBus.Publish(new ComboChangedEvent
            {
                Combo = CurrentCombo,
                OnFire = IsOnFire
            });
        }

        public void ResetCombo()
        {
            if (CurrentCombo > 0)
            {
                CurrentCombo = 0;
                PerfectStreak = 0;

                EventBus.Publish(new ComboChangedEvent
                {
                    Combo = 0,
                    OnFire = false
                });
            }
        }

        public float GetComboMultiplier()
        {
            float mult = 1f + CurrentCombo * _config.comboMultiplierPerStep;
            return Mathf.Min(mult, _config.maxComboMultiplier);
        }

        public int GetStreakBonus()
        {
            if (PerfectStreak < _config.onFireThreshold)
                return 0;

            int levelsAboveThreshold = PerfectStreak - _config.onFireThreshold;
            return _config.streakBonusPoints + levelsAboveThreshold * _config.streakBonusPerLevel;
        }

        public void Reset()
        {
            CurrentCombo = 0;
            PerfectStreak = 0;
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<ComboTracker>();
        }
    }
}
