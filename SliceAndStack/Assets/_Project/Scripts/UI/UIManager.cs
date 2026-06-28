using System.Collections.Generic;
using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private MainMenuScreen _mainMenuScreen;
        [SerializeField] private HUDScreen _hudScreen;
        [SerializeField] private PauseScreen _pauseScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private ResultsScreen _resultsScreen;
        [SerializeField] private SettingsScreen _settingsScreen;
        [SerializeField] private ShopScreen _shopScreen;
        [SerializeField] private LeaderboardScreen _leaderboardScreen;

        private readonly Stack<UIScreen> _screenStack = new();
        private UIScreen _currentScreen;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            // Hide all screens
            HideAll();

            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            ServiceLocator.Unregister<UIManager>();
        }

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            switch (evt.NewState)
            {
                case GameState.MainMenu:
                    ShowScreen(_mainMenuScreen);
                    break;

                case GameState.Playing:
                    ShowScreen(_hudScreen);
                    break;

                case GameState.Paused:
                    PushScreen(_pauseScreen);
                    break;

                case GameState.GameOver:
                    ShowScreen(_gameOverScreen);
                    break;

                case GameState.Results:
                    ShowScreen(_resultsScreen);
                    break;
            }
        }

        public void ShowScreen(UIScreen screen)
        {
            if (screen == null) return;

            HideAll();
            _screenStack.Clear();
            _currentScreen = screen;
            screen.Show();
        }

        public void PushScreen(UIScreen screen)
        {
            if (screen == null) return;

            if (_currentScreen != null)
                _screenStack.Push(_currentScreen);

            _currentScreen = screen;
            screen.Show();
        }

        public void PopScreen()
        {
            if (_currentScreen != null)
                _currentScreen.Hide();

            if (_screenStack.Count > 0)
            {
                _currentScreen = _screenStack.Pop();
                _currentScreen.Show();
            }
        }

        public void ShowSettings()
        {
            PushScreen(_settingsScreen);
        }

        public void ShowShop()
        {
            PushScreen(_shopScreen);
        }

        public void ShowLeaderboard()
        {
            PushScreen(_leaderboardScreen);
        }

        public void Back()
        {
            PopScreen();
        }

        private void HideAll()
        {
            if (_mainMenuScreen != null) _mainMenuScreen.Hide();
            if (_hudScreen != null) _hudScreen.Hide();
            if (_pauseScreen != null) _pauseScreen.Hide();
            if (_gameOverScreen != null) _gameOverScreen.Hide();
            if (_resultsScreen != null) _resultsScreen.Hide();
            if (_settingsScreen != null) _settingsScreen.Hide();
            if (_shopScreen != null) _shopScreen.Hide();
            if (_leaderboardScreen != null) _leaderboardScreen.Hide();
        }
    }
}
