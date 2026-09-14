using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SliceAndStack.Core;
using SliceAndStack.Utils;

namespace SliceAndStack.UI
{
    public class HUDScreen : UIScreen
    {
        [Header("Score")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _comboText;
        [SerializeField] private TextMeshProUGUI _qualityPopup;

        [Header("Height")]
        [SerializeField] private TextMeshProUGUI _heightText;
        [SerializeField] private Slider _heightSlider;

        [Header("Stability")]
        [SerializeField] private Slider _stabilityBar;
        [SerializeField] private Image _stabilityFill;
        [SerializeField] private Image _vignetteOverlay;

        [Header("Streak")]
        [SerializeField] private GameObject _onFireOverlay;
        [SerializeField] private GameObject _rainbowOverlay;

        [Header("New Record")]
        [SerializeField] private GameObject _newRecordBanner;

        [Header("Pause")]
        [SerializeField] private Button _pauseButton;

        private Color _normalStabilityColor = Color.green;
        private Color _warningColor = Color.yellow;
        private Color _alertColor = new Color(1f, 0.5f, 0f);
        private Color _criticalColor = Color.red;

        protected override void Awake()
        {
            base.Awake();
            _pauseButton?.onClick.AddListener(OnPauseClicked);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
            EventBus.Subscribe<ComboChangedEvent>(OnComboChanged);
            EventBus.Subscribe<StabilityChangedEvent>(OnStabilityChanged);
            EventBus.Subscribe<SlicePerformedEvent>(OnSlicePerformed);
            EventBus.Subscribe<NewRecordEvent>(OnNewRecord);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
            EventBus.Unsubscribe<ComboChangedEvent>(OnComboChanged);
            EventBus.Unsubscribe<StabilityChangedEvent>(OnStabilityChanged);
            EventBus.Unsubscribe<SlicePerformedEvent>(OnSlicePerformed);
            EventBus.Unsubscribe<NewRecordEvent>(OnNewRecord);
        }

        public override void Show()
        {
            base.Show();
            ResetHUD();
        }

        private void ResetHUD()
        {
            if (_scoreText != null) _scoreText.text = "0";
            if (_comboText != null) _comboText.text = "";
            if (_heightText != null) _heightText.text = "0.0m";
            if (_stabilityBar != null) _stabilityBar.value = 1f;
            if (_onFireOverlay != null) _onFireOverlay.SetActive(false);
            if (_rainbowOverlay != null) _rainbowOverlay.SetActive(false);
            if (_newRecordBanner != null) _newRecordBanner.SetActive(false);
            if (_vignetteOverlay != null) _vignetteOverlay.enabled = false;
        }

        private void Update()
        {
            if (ServiceLocator.TryGet<Stacking.TowerManager>(out var tower))
            {
                if (_heightText != null)
                    _heightText.text = tower.CurrentTowerHeight.FormatHeight();

                if (_stabilityBar != null)
                    _stabilityBar.value = Mathf.Lerp(_stabilityBar.value, tower.GetStability(), Time.deltaTime * 5f);
            }
        }

        private void OnScoreChanged(ScoreChangedEvent evt)
        {
            if (_scoreText != null)
                _scoreText.text = evt.CurrentScore.FormatScore();
        }

        private void OnComboChanged(ComboChangedEvent evt)
        {
            if (_comboText != null)
            {
                _comboText.text = evt.Combo > 1 ? $"x{evt.Combo}" : "";
                // DOTween: _comboText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
            }

            if (_onFireOverlay != null)
                _onFireOverlay.SetActive(evt.OnFire);

            if (_rainbowOverlay != null && ServiceLocator.TryGet<Scoring.ComboTracker>(out var combo))
                _rainbowOverlay.SetActive(combo.IsRainbow);
        }

        private void OnStabilityChanged(StabilityChangedEvent evt)
        {
            if (_stabilityFill != null)
            {
                _stabilityFill.color = evt.Level switch
                {
                    StabilityLevel.Normal => _normalStabilityColor,
                    StabilityLevel.Warning => _warningColor,
                    StabilityLevel.Alert => _alertColor,
                    StabilityLevel.Critical => _criticalColor,
                    _ => _normalStabilityColor
                };
            }

            if (_vignetteOverlay != null)
            {
                _vignetteOverlay.enabled = evt.Level == StabilityLevel.Critical;
                if (evt.Level == StabilityLevel.Critical)
                    _vignetteOverlay.color = new Color(1f, 0f, 0f, 0.3f);
            }
        }

        private void OnSlicePerformed(SlicePerformedEvent evt)
        {
            if (_qualityPopup == null || evt.Quality == SliceQuality.Miss) return;

            _qualityPopup.text = evt.Quality.ToString().ToUpper();
            _qualityPopup.color = evt.Quality switch
            {
                SliceQuality.Perfect => Color.yellow,
                SliceQuality.Great => Color.cyan,
                _ => Color.white
            };
            _qualityPopup.gameObject.SetActive(true);

            // DOTween: animate scale + fade out
            CancelInvoke(nameof(HideQualityPopup));
            Invoke(nameof(HideQualityPopup), 0.8f);
        }

        private void HideQualityPopup()
        {
            if (_qualityPopup != null)
                _qualityPopup.gameObject.SetActive(false);
        }

        private void OnNewRecord(NewRecordEvent evt)
        {
            if (_newRecordBanner != null)
            {
                _newRecordBanner.SetActive(true);
                // DOTween: animate in then auto-hide after 2s
                CancelInvoke(nameof(HideNewRecord));
                Invoke(nameof(HideNewRecord), 3f);
            }
        }

        private void HideNewRecord()
        {
            if (_newRecordBanner != null)
                _newRecordBanner.SetActive(false);
        }

        private void OnPauseClicked()
        {
            GameManager.Instance?.PauseGame();
        }
    }
}
