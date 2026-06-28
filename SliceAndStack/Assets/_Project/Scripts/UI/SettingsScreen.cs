using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SliceAndStack.Core;

namespace SliceAndStack.UI
{
    public class SettingsScreen : UIScreen
    {
        [Header("Audio")]
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Toggle _sfxMuteToggle;
        [SerializeField] private Toggle _musicMuteToggle;

        [Header("Haptics")]
        [SerializeField] private Toggle _hapticsToggle;

        [Header("Other")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _resetDataButton;
        [SerializeField] private TextMeshProUGUI _versionText;

        protected override void Awake()
        {
            base.Awake();

            _sfxVolumeSlider?.onValueChanged.AddListener(OnSfxVolumeChanged);
            _musicVolumeSlider?.onValueChanged.AddListener(OnMusicVolumeChanged);
            _sfxMuteToggle?.onValueChanged.AddListener(OnSfxMuteChanged);
            _musicMuteToggle?.onValueChanged.AddListener(OnMusicMuteChanged);
            _hapticsToggle?.onValueChanged.AddListener(OnHapticsChanged);
            _backButton?.onClick.AddListener(OnBackClicked);
            _resetDataButton?.onClick.AddListener(OnResetDataClicked);
        }

        public override void Show()
        {
            base.Show();
            LoadSettings();

            if (_versionText != null)
                _versionText.text = $"v{Application.version}";
        }

        private void LoadSettings()
        {
            if (!ServiceLocator.TryGet<Persistence.SaveManager>(out var save)) return;

            var settings = save.Data.settings;

            if (_sfxVolumeSlider != null) _sfxVolumeSlider.value = settings.sfxVolume;
            if (_musicVolumeSlider != null) _musicVolumeSlider.value = settings.musicVolume;
            if (_sfxMuteToggle != null) _sfxMuteToggle.isOn = !settings.sfxMuted;
            if (_musicMuteToggle != null) _musicMuteToggle.isOn = !settings.musicMuted;
            if (_hapticsToggle != null) _hapticsToggle.isOn = settings.hapticsEnabled;
        }

        private void OnSfxVolumeChanged(float value)
        {
            if (ServiceLocator.TryGet<Audio.AudioManager>(out var audio))
                audio.SetSfxVolume(value);

            UpdateSaveSettings(s => s.sfxVolume = value);
        }

        private void OnMusicVolumeChanged(float value)
        {
            if (ServiceLocator.TryGet<Audio.AudioManager>(out var audio))
                audio.SetMusicVolume(value);

            UpdateSaveSettings(s => s.musicVolume = value);
        }

        private void OnSfxMuteChanged(bool isOn)
        {
            if (ServiceLocator.TryGet<Audio.AudioManager>(out var audio))
                audio.SetSfxMuted(!isOn);

            UpdateSaveSettings(s => s.sfxMuted = !isOn);
        }

        private void OnMusicMuteChanged(bool isOn)
        {
            if (ServiceLocator.TryGet<Audio.AudioManager>(out var audio))
                audio.SetMusicMuted(!isOn);

            UpdateSaveSettings(s => s.musicMuted = !isOn);
        }

        private void OnHapticsChanged(bool isOn)
        {
            if (ServiceLocator.TryGet<Haptics.HapticManager>(out var haptic))
                haptic.SetEnabled(isOn);
        }

        private void UpdateSaveSettings(System.Action<Persistence.SettingsData> modifier)
        {
            if (!ServiceLocator.TryGet<Persistence.SaveManager>(out var save)) return;
            modifier(save.Data.settings);
            save.Save();
        }

        private void OnBackClicked()
        {
            if (ServiceLocator.TryGet<UIManager>(out var ui))
                ui.Back();
        }

        private void OnResetDataClicked()
        {
            if (ServiceLocator.TryGet<Persistence.SaveManager>(out var save))
            {
                save.DeleteSave();
                LoadSettings();
            }
        }
    }
}
