using UnityEngine;
using SliceAndStack.Core;
using SliceAndStack.Stacking;

namespace SliceAndStack.Scoring
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private ScoreConfig _config;

        public int CurrentScore { get; private set; }

        private ScoreCalculator _calculator;
        private ComboTracker _comboTracker;

        private void Awake()
        {
            ServiceLocator.Register(this);
            _calculator = new ScoreCalculator(_config);
        }

        private void Start()
        {
            _comboTracker = ServiceLocator.Get<ComboTracker>();
            EventBus.Subscribe<SlicePerformedEvent>(OnSlicePerformed);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<SlicePerformedEvent>(OnSlicePerformed);
            ServiceLocator.Unregister<ScoreManager>();
        }

        private void OnSlicePerformed(SlicePerformedEvent evt)
        {
            if (evt.Quality == SliceQuality.Miss)
            {
                _comboTracker.ResetCombo();
                return;
            }

            _comboTracker.RegisterHit(evt.Quality);

            float towerHeight = 0f;
            if (ServiceLocator.TryGet<TowerManager>(out var tower))
                towerHeight = tower.CurrentTowerHeight;

            // Get base points from the sliceable data (default 100 if unavailable)
            int basePoints = 100;

            int points = _calculator.Calculate(
                basePoints,
                evt.Quality,
                _comboTracker.GetComboMultiplier(),
                towerHeight,
                _comboTracker.GetStreakBonus()
            );

            AddScore(points);
        }

        public void AddScore(int points)
        {
            CurrentScore += points;

            EventBus.Publish(new ScoreChangedEvent
            {
                CurrentScore = CurrentScore,
                PointsAdded = points,
                Combo = _comboTracker.CurrentCombo
            });

            // Check for new record mid-game
            if (ServiceLocator.TryGet<Persistence.SaveManager>(out var save))
            {
                if (CurrentScore > save.Data.highScore)
                {
                    EventBus.Publish(new NewRecordEvent { Score = CurrentScore });
                }
            }
        }

        public void DoubleScore()
        {
            int bonus = CurrentScore;
            CurrentScore *= 2;

            EventBus.Publish(new ScoreChangedEvent
            {
                CurrentScore = CurrentScore,
                PointsAdded = bonus,
                Combo = _comboTracker?.CurrentCombo ?? 0
            });
        }

        public void Reset()
        {
            CurrentScore = 0;
        }
    }
}
