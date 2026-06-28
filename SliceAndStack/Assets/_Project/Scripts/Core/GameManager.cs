using UnityEngine;
using UnityEngine.SceneManagement;

namespace SliceAndStack.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject _towerBasePrefab;

        public GameState CurrentState { get; private set; } = GameState.Boot;
        public int GamesPlayedThisSession { get; private set; }
        public float SessionStartTime { get; private set; }
        public int CurrentScore => ServiceLocator.TryGet<Scoring.ScoreManager>(out var sm) ? sm.CurrentScore : 0;

        private bool _reviveUsedThisGame;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SessionStartTime = Time.realtimeSinceStartup;
        }

        private void Start()
        {
            TransitionTo(GameState.MainMenu);
        }

        public void TransitionTo(GameState newState)
        {
            var previous = CurrentState;
            if (previous == newState) return;

            OnExitState(previous);
            CurrentState = newState;
            OnEnterState(newState);

            EventBus.Publish(new GameStateChangedEvent
            {
                PreviousState = previous,
                NewState = newState
            });
        }

        private void OnExitState(GameState state)
        {
            switch (state)
            {
                case GameState.Paused:
                    Time.timeScale = 1f;
                    break;
            }
        }

        private void OnEnterState(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;

                case GameState.Playing:
                    if (CurrentState != GameState.Reviving)
                        Time.timeScale = 1f;
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;

                case GameState.GameOver:
                    GamesPlayedThisSession++;
                    break;

                case GameState.Reviving:
                    _reviveUsedThisGame = true;
                    break;
            }
        }

        public void StartGame()
        {
            _reviveUsedThisGame = false;

            if (ServiceLocator.TryGet<Scoring.ScoreManager>(out var scoreManager))
                scoreManager.Reset();
            if (ServiceLocator.TryGet<Scoring.ComboTracker>(out var comboTracker))
                comboTracker.Reset();
            if (ServiceLocator.TryGet<Stacking.TowerManager>(out var towerManager))
                towerManager.ResetTower();
            if (ServiceLocator.TryGet<Spawning.SpawnManager>(out var spawnManager))
                spawnManager.Reset();
            if (ServiceLocator.TryGet<Difficulty.DifficultyManager>(out var difficultyManager))
                difficultyManager.Reset();

            TransitionTo(GameState.Playing);
        }

        public void PauseGame()
        {
            if (CurrentState == GameState.Playing)
                TransitionTo(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
                TransitionTo(GameState.Playing);
        }

        public void TriggerGameOver()
        {
            if (CurrentState != GameState.Playing) return;

            float towerHeight = 0f;
            if (ServiceLocator.TryGet<Stacking.TowerManager>(out var tm))
                towerHeight = tm.CurrentTowerHeight;

            bool isNewRecord = false;
            if (ServiceLocator.TryGet<Persistence.SaveManager>(out var saveManager))
                isNewRecord = saveManager.Data.TrySetHighScore(CurrentScore);

            EventBus.Publish(new GameOverEvent
            {
                FinalScore = CurrentScore,
                TowerHeight = towerHeight,
                IsNewRecord = isNewRecord
            });

            TransitionTo(GameState.GameOver);
        }

        public bool CanRevive()
        {
            return !_reviveUsedThisGame;
        }

        public void Revive()
        {
            if (!CanRevive()) return;
            TransitionTo(GameState.Reviving);

            if (ServiceLocator.TryGet<Stacking.TowerManager>(out var tm))
                tm.RestorePreCollapseState();

            EventBus.Publish(new ReviveEvent());
            TransitionTo(GameState.Playing);
        }

        public void ShowResults()
        {
            TransitionTo(GameState.Results);
        }

        public void ReturnToMenu()
        {
            TransitionTo(GameState.MainMenu);
        }

        public void RestartGame()
        {
            StartGame();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
