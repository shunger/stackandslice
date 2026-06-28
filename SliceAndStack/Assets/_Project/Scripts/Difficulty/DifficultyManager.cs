using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Difficulty
{
    public class DifficultyManager : MonoBehaviour
    {
        [SerializeField] private DifficultyConfig _config;

        public float CurrentSpawnInterval { get; private set; }
        public float CurrentFallSpeed { get; private set; }
        public float CurrentSliceZoneSize { get; private set; }
        public float CurrentWindStrength { get; private set; }
        public float CurrentWindDirection { get; private set; }

        private float _targetWindDirection;
        private float _windVelocity;
        private float _windChangeTimer;
        private int _lastEvaluatedScore = -1;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
            Reset();
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
            ServiceLocator.Unregister<DifficultyManager>();
        }

        private void Update()
        {
            UpdateWind();
        }

        private void OnScoreChanged(ScoreChangedEvent evt)
        {
            EvaluateDifficulty(evt.CurrentScore);
        }

        private void EvaluateDifficulty(int score)
        {
            if (score == _lastEvaluatedScore) return;
            _lastEvaluatedScore = score;

            CurrentSpawnInterval = _config.spawnInterval.Evaluate(score);
            CurrentFallSpeed = _config.fallSpeed.Evaluate(score);
            CurrentSliceZoneSize = _config.sliceZoneSize.Evaluate(score);
            CurrentWindStrength = _config.windStrength.Evaluate(score);

            EventBus.Publish(new DifficultyChangedEvent
            {
                SpawnInterval = CurrentSpawnInterval,
                FallSpeed = CurrentFallSpeed,
                SliceZoneSize = CurrentSliceZoneSize,
                WindStrength = CurrentWindStrength
            });
        }

        private void UpdateWind()
        {
            if (CurrentWindStrength <= 0f) return;

            _windChangeTimer += Time.deltaTime;
            if (_windChangeTimer >= _config.windChangeInterval)
            {
                _windChangeTimer = 0f;
                _targetWindDirection = Random.Range(-1f, 1f);
            }

            CurrentWindDirection = Mathf.SmoothDamp(
                CurrentWindDirection, _targetWindDirection,
                ref _windVelocity, _config.windSmoothTime);
        }

        public Vector2 GetWindForce()
        {
            return new Vector2(CurrentWindDirection * CurrentWindStrength, 0f);
        }

        public void Reset()
        {
            _lastEvaluatedScore = -1;
            CurrentSpawnInterval = _config.spawnInterval.Evaluate(0);
            CurrentFallSpeed = _config.fallSpeed.Evaluate(0);
            CurrentSliceZoneSize = _config.sliceZoneSize.Evaluate(0);
            CurrentWindStrength = 0f;
            CurrentWindDirection = 0f;
            _windChangeTimer = 0f;

            EventBus.Publish(new DifficultyChangedEvent
            {
                SpawnInterval = CurrentSpawnInterval,
                FallSpeed = CurrentFallSpeed,
                SliceZoneSize = CurrentSliceZoneSize,
                WindStrength = CurrentWindStrength
            });
        }
    }
}
