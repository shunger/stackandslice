using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SliceAndStack.Core;

namespace SliceAndStack.UI
{
    public class MainMenuScreen : UIScreen
    {
        [Header("UI Elements")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _leaderboardButton;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _highScoreText;

        protected override void Awake()
        {
            base.Awake();

            _playButton?.onClick.AddListener(OnPlayClicked);
            _settingsButton?.onClick.AddListener(OnSettingsClicked);
            _shopButton?.onClick.AddListener(OnShopClicked);
            _leaderboardButton?.onClick.AddListener(OnLeaderboardClicked);
        }

        public override void Show()
        {
            base.Show();
            UpdateHighScore();
        }

        private void UpdateHighScore()
        {
            if (_highScoreText != null && ServiceLocator.TryGet<Persistence.SaveManager>(out var save))
            {
                _highScoreText.text = $"Best: {save.Data.highScore:N0}";
            }
        }

        private void OnPlayClicked()
        {
            if (ServiceLocator.TryGet<Audio.AudioManager>(out var audio))
                audio.PlaySfx("ui_click");

            GameManager.Instance?.StartGame();
        }

        private void OnSettingsClicked()
        {
            if (ServiceLocator.TryGet<UIManager>(out var ui))
                ui.ShowSettings();
        }

        private void OnShopClicked()
        {
            if (ServiceLocator.TryGet<UIManager>(out var ui))
                ui.ShowShop();
        }

        private void OnLeaderboardClicked()
        {
            if (ServiceLocator.TryGet<UIManager>(out var ui))
                ui.ShowLeaderboard();
        }
    }
}
