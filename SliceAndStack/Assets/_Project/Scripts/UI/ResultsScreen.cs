using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SliceAndStack.Core;
using SliceAndStack.Utils;

namespace SliceAndStack.UI
{
    public class ResultsScreen : UIScreen
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _finalScoreText;
        [SerializeField] private TextMeshProUGUI _towerHeightText;
        [SerializeField] private TextMeshProUGUI _bestScoreText;
        [SerializeField] private GameObject _newRecordBadge;

        [Header("Buttons")]
        [SerializeField] private Button _playAgainButton;
        [SerializeField] private Button _doubleScoreButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _shareButton;

        private bool _scoreDoubled;

        protected override void Awake()
        {
            base.Awake();
            _playAgainButton?.onClick.AddListener(OnPlayAgainClicked);
            _doubleScoreButton?.onClick.AddListener(OnDoubleScoreClicked);
            _menuButton?.onClick.AddListener(OnMenuClicked);
            _shareButton?.onClick.AddListener(OnShareClicked);
        }

        public override void Show()
        {
            base.Show();
            _scoreDoubled = false;

            var gm = GameManager.Instance;
            int score = gm != null ? gm.CurrentScore : 0;

            if (_finalScoreText != null)
                _finalScoreText.text = score.FormatScore();

            if (ServiceLocator.TryGet<Stacking.TowerManager>(out var tower))
            {
                if (_towerHeightText != null)
                    _towerHeightText.text = tower.CurrentTowerHeight.FormatHeight();
            }

            if (ServiceLocator.TryGet<Persistence.SaveManager>(out var save))
            {
                if (_bestScoreText != null)
                    _bestScoreText.text = $"Best: {save.Data.highScore:N0}";

                if (_newRecordBadge != null)
                    _newRecordBadge.SetActive(score >= save.Data.highScore);

                save.RecordGameResult(score, tower?.CurrentTowerHeight ?? 0f);
            }

            // Double score button
            if (_doubleScoreButton != null)
            {
                _doubleScoreButton.gameObject.SetActive(true);
                if (ServiceLocator.TryGet<Ads.AdManager>(out var adManager))
                    _doubleScoreButton.interactable = adManager.IsRewardedReady();
            }

            // Check cosmetic unlocks
            if (ServiceLocator.TryGet<Cosmetics.CosmeticManager>(out var cosmetics))
                cosmetics.CheckUnlocks();
        }

        private void OnPlayAgainClicked()
        {
            GameManager.Instance?.RestartGame();
        }

        private void OnDoubleScoreClicked()
        {
            if (_scoreDoubled) return;

            if (!ServiceLocator.TryGet<Ads.AdManager>(out var adManager)) return;

            adManager.ShowRewarded(RewardType.DoubleScore, success =>
            {
                if (success)
                {
                    _scoreDoubled = true;
                    if (_doubleScoreButton != null)
                        _doubleScoreButton.interactable = false;

                    int newScore = GameManager.Instance != null ? GameManager.Instance.CurrentScore : 0;
                    if (_finalScoreText != null)
                        _finalScoreText.text = newScore.FormatScore();

                    // Re-save with doubled score
                    if (ServiceLocator.TryGet<Persistence.SaveManager>(out var save))
                        save.RecordGameResult(newScore, 0f);
                }
            });
        }

        private void OnMenuClicked()
        {
            GameManager.Instance?.ReturnToMenu();
        }

        private void OnShareClicked()
        {
            int score = GameManager.Instance != null ? GameManager.Instance.CurrentScore : 0;
            string message = $"I scored {score:N0} in Slice & Stack! Can you beat my tower?";

            // NativeShare plugin required for native sharing.
            // Install from: https://github.com/yasirkula/UnityNativeShare
            // Then uncomment:
            // new NativeShare().SetText(message).Share();
            Debug.Log($"[Share] {message}");
        }
    }
}
