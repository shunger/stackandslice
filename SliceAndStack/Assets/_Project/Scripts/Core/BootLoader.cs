using UnityEngine;
using UnityEngine.SceneManagement;

namespace SliceAndStack.Core
{
    public class BootLoader : MonoBehaviour
    {
        [SerializeField] private string _nextScene = Utils.Constants.SCENE_GAME;

        private void Start()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Initialize ad system
            if (ServiceLocator.TryGet<Ads.AdManager>(out var adManager))
                adManager.Initialize();

            // Load settings
            if (ServiceLocator.TryGet<Persistence.SaveManager>(out var save))
            {
                var settings = save.Data.settings;

                if (ServiceLocator.TryGet<Audio.AudioManager>(out var audio))
                {
                    audio.SetSfxVolume(settings.sfxVolume);
                    audio.SetMusicVolume(settings.musicVolume);
                    audio.SetSfxMuted(settings.sfxMuted);
                    audio.SetMusicMuted(settings.musicMuted);
                }
            }

            // If using multiple scenes, load next scene
            // SceneManager.LoadScene(_nextScene);

            // For single-scene setup, transition to menu
            if (GameManager.Instance != null)
                GameManager.Instance.TransitionTo(GameState.MainMenu);
        }
    }
}
