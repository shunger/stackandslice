using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SliceAndStack.Core;

namespace SliceAndStack.UI
{
    public class GameOverScreen : UIScreen
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _gameOverText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _nearMissText;
        [SerializeField] private TextMeshProUGUI _stabilityText;

        [Header("Buttons")]
        [SerializeField] private Button _reviveButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private TextMeshProUGUI _reviveButtonLabel;

        private int _finalScore;
        private float _finalHeight;

        protected override void Awake()
        {
            base.Awake();
            _reviveButton?.onClick.AddListener(OnReviveClicked);
            _continueButton?.onClick.AddListener(OnContinueClicked);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameOverEvent>(OnGameOver);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        }

        private void OnGameOver(GameOverEvent evt)
        {
            _finalScore = evt.FinalScore;
            _finalHeight = evt.TowerHeight;
        }

        public override void Show()
        {
            base.Show();

            if (_scoreText != null)
                _scoreText.text = _finalScore.FormatScore();

            // Near-miss display
            if (_nearMissText != null && ServiceLocator.TryGet<Persistence.SaveManager>(out var save))
            {
                int diff = save.Data.highScore - _finalScore;
                if (diff > 0 && diff < 100)
                    _nearMissText.text = $"Just {diff} from record!";
                else
                    _nearMissText.text = "";
            }

            // Stability at collapse
            if (_stabilityText != null && ServiceLocator.TryGet<Stacking.TowerManager>(out var tower))
            {
                float stability = tower.GetStability();
                _stabilityText.text = $"Stability: {Mathf.RoundToInt(stability * 100)}%";
            }

            // Revive button
            bool canRevive = GameManager.Instance != null && GameManager.Instance.CanRevive();
            if (_reviveButton != null)
            {
                _reviveButton.gameObject.SetActive(canRevive);
                if (canRevive && ServiceLocator.TryGet<Ads.AdManager>(out var adManager))
                    _reviveButton.interactable = adManager.IsRewardedReady();
            }
        }

        private void OnReviveClicked()
        {
            if (!ServiceLocator.TryGet<Ads.AdManager>(out var adManager)) return;

            adManager.ShowRewarded(RewardType.Revive, success =>
            {
                if (success)
                {
                    // Revive handled by RewardHandler
                    if (ServiceLocator.TryGet<UIManager>(out var ui))
                        ui.ShowScreen(null); // Will transition via GameStateChanged
                }
            });
        }

        private void OnContinueClicked()
        {
            // Check if should show interstitial
            if (ServiceLocator.TryGet<Ads.AdManager>(out var adManager) && adManager.ShouldShowInterstitial())
            {
                adManager.ShowInterstitial(() =>
                {
                    GameManager.Instance?.ShowResults();
                });
            }
            else
            {
                GameManager.Instance?.ShowResults();
            }
        }
    }
}
