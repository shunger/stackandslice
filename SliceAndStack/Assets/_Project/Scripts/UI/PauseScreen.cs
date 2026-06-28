using UnityEngine;
using UnityEngine.UI;
using SliceAndStack.Core;

namespace SliceAndStack.UI
{
    public class PauseScreen : UIScreen
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

        protected override void Awake()
        {
            base.Awake();
            _resumeButton?.onClick.AddListener(OnResumeClicked);
            _settingsButton?.onClick.AddListener(OnSettingsClicked);
            _quitButton?.onClick.AddListener(OnQuitClicked);
        }

        private void OnResumeClicked()
        {
            GameManager.Instance?.ResumeGame();

            if (ServiceLocator.TryGet<UIManager>(out var ui))
                ui.Back();
        }

        private void OnSettingsClicked()
        {
            if (ServiceLocator.TryGet<UIManager>(out var ui))
                ui.ShowSettings();
        }

        private void OnQuitClicked()
        {
            GameManager.Instance?.ResumeGame();
            GameManager.Instance?.ReturnToMenu();
        }
    }
}
