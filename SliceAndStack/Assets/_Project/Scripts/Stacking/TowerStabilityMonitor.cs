using System.Collections.Generic;
using UnityEngine;
using SliceAndStack.Core;
using SliceAndStack.Utils;

namespace SliceAndStack.Stacking
{
    public class TowerStabilityMonitor : MonoBehaviour
    {
        [SerializeField] private StackingConfig _config;

        public float CurrentStability { get; private set; } = 1f;
        public StabilityLevel CurrentLevel { get; private set; } = StabilityLevel.Normal;

        private List<StackedPiece> _pieces;
        private float _baseCenterX;
        private float _collapseTimer;
        private StabilityLevel _previousLevel;
        private float _smoothedStability = 1f;
        private float _stabilityVelocity;

        public void SetConfig(StackingConfig config)
        {
            _config = config;
        }

        public void Initialize(List<StackedPiece> pieces, float baseCenterX)
        {
            _pieces = pieces;
            _baseCenterX = baseCenterX;
            CurrentStability = 1f;
            _smoothedStability = 1f;
            _collapseTimer = 0f;
            CurrentLevel = StabilityLevel.Normal;
            _previousLevel = StabilityLevel.Normal;
        }

        private void FixedUpdate()
        {
            if (_config == null || _pieces == null || _pieces.Count == 0) return;

            float rawStability = CalculateStability();
            _smoothedStability = Mathf.SmoothDamp(_smoothedStability, rawStability,
                ref _stabilityVelocity, 0.1f);
            CurrentStability = Mathf.Clamp01(_smoothedStability);

            UpdateStabilityLevel();
            // Collapse detection disabled during development
            // CheckForCollapse();
        }

        private float CalculateStability()
        {
            if (_config == null) return 1f;

            // Gather valid rigidbodies
            var bodies = new List<Rigidbody2D>();
            float maxAngular = 0f;

            for (int i = _pieces.Count - 1; i >= 0; i--)
            {
                if (_pieces[i] == null || _pieces[i].Rb == null)
                {
                    _pieces.RemoveAt(i);
                    continue;
                }

                bodies.Add(_pieces[i].Rb);
                float angular = Mathf.Abs(_pieces[i].Rb.angularVelocity);
                if (angular > maxAngular) maxAngular = angular;
            }

            if (bodies.Count == 0) return 1f;

            // Center of mass offset stability
            Vector2 com = MathUtils.CalculateCenterOfMass(bodies.ToArray());
            float comOffset = Mathf.Abs(com.x - _baseCenterX);
            float comStability = 1f - Mathf.Clamp01(comOffset / _config.maxCenterOfMassOffset);

            // Angular velocity stability
            float angularStability = 1f - Mathf.Clamp01(maxAngular / _config.maxAngularVelocity);

            // Weighted combination
            float combined = comStability * _config.comWeightInStability +
                            angularStability * _config.angularWeightInStability;

            return combined;
        }

        private void UpdateStabilityLevel()
        {
            StabilityLevel newLevel;
            if (CurrentStability >= _config.warningThreshold)
                newLevel = StabilityLevel.Normal;
            else if (CurrentStability >= _config.alertThreshold)
                newLevel = StabilityLevel.Warning;
            else if (CurrentStability >= _config.criticalThreshold)
                newLevel = StabilityLevel.Alert;
            else
                newLevel = StabilityLevel.Critical;

            if (newLevel != _previousLevel)
            {
                CurrentLevel = newLevel;
                _previousLevel = newLevel;

                EventBus.Publish(new StabilityChangedEvent
                {
                    Stability = CurrentStability,
                    Level = CurrentLevel
                });
            }
        }

        private void CheckForCollapse()
        {
            // Check if any piece fell off screen
            foreach (var piece in _pieces)
            {
                if (piece != null && piece.transform.position.y < _config.fallOffY)
                {
                    Debug.Log($"[TowerStability] Collapse: piece at Y={piece.transform.position.y:F2} below fallOffY={_config.fallOffY}");
                    TriggerCollapse();
                    return;
                }
            }

            // Check sustained zero stability
            if (CurrentStability <= 0f)
            {
                _collapseTimer += Time.fixedDeltaTime;
                if (_collapseTimer >= _config.collapseConfirmTime)
                {
                    Debug.Log($"[TowerStability] Collapse: stability at 0 for {_collapseTimer:F2}s");
                    TriggerCollapse();
                }
            }
            else
            {
                _collapseTimer = 0f;
            }
        }

        private void TriggerCollapse()
        {
            float height = 0f;
            foreach (var piece in _pieces)
            {
                if (piece != null)
                {
                    float pieceHeight = piece.transform.position.y - Constants.TOWER_BASE_Y;
                    if (pieceHeight > height) height = pieceHeight;
                }
            }

            EventBus.Publish(new TowerCollapsedEvent
            {
                FinalHeight = height,
                FinalScore = GameManager.Instance != null ? GameManager.Instance.CurrentScore : 0
            });

            if (GameManager.Instance != null)
                GameManager.Instance.TriggerGameOver();
        }

        public void Reset()
        {
            CurrentStability = 1f;
            _smoothedStability = 1f;
            _collapseTimer = 0f;
            CurrentLevel = StabilityLevel.Normal;
            _previousLevel = StabilityLevel.Normal;
        }
    }
}
