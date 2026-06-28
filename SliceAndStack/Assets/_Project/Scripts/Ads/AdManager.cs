using System;
using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Ads
{
    public class AdManager : MonoBehaviour
    {
        [SerializeField] private AdConfig _config;

        public bool IsInitialized { get; private set; }
        public bool HasConsentBeenGiven { get; private set; }

        private int _gamesSinceLastInterstitial;
        private float _lastInterstitialTime = -999f;
        private Action<bool> _rewardCallback;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            ServiceLocator.Unregister<AdManager>();
        }

        public void Initialize()
        {
            // AdMob initialization would go here:
            // MobileAds.Initialize(status => { IsInitialized = true; });
            Debug.Log("[AdManager] AdMob initialization (stub - install Google Mobile Ads Unity plugin)");
            IsInitialized = true;
            PreloadInterstitial();
            PreloadRewarded();
        }

        public void SetConsent(bool consent)
        {
            HasConsentBeenGiven = consent;
            // Apply consent to AdMob SDK
        }

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            if (evt.NewState == GameState.GameOver)
            {
                _gamesSinceLastInterstitial++;
            }
        }

        // --- Interstitial ---

        public bool ShouldShowInterstitial()
        {
            if (!IsInitialized) return false;

            float sessionDuration = Time.realtimeSinceStartup -
                (GameManager.Instance != null ? GameManager.Instance.SessionStartTime : 0f);
            if (sessionDuration < _config.minSessionDurationForInterstitial) return false;

            if (_gamesSinceLastInterstitial < _config.interstitialGameInterval) return false;

            float timeSinceLast = Time.realtimeSinceStartup - _lastInterstitialTime;
            if (timeSinceLast < _config.minTimeBetweenInterstitials) return false;

            return true;
        }

        public void ShowInterstitial(Action onComplete = null)
        {
            if (!ShouldShowInterstitial())
            {
                onComplete?.Invoke();
                return;
            }

            Debug.Log("[AdManager] Showing interstitial (stub)");
            // In production:
            // _interstitialAd.Show();
            // Register OnAdFullScreenContentClosed to call onComplete

            _gamesSinceLastInterstitial = 0;
            _lastInterstitialTime = Time.realtimeSinceStartup;
            onComplete?.Invoke();
        }

        private void PreloadInterstitial()
        {
            Debug.Log($"[AdManager] Preloading interstitial: {_config.InterstitialId}");
            // InterstitialAd.Load(_config.InterstitialId, request, (ad, error) => { ... });
        }

        // --- Rewarded ---

        public void ShowRewarded(RewardType rewardType, Action<bool> callback)
        {
            _rewardCallback = callback;
            Debug.Log($"[AdManager] Showing rewarded ad for: {rewardType} (stub)");

            // In production:
            // _rewardedAd.Show(reward => { OnRewardEarned(rewardType); });

            // Stub: simulate reward
            OnRewardEarned(rewardType);
        }

        private void OnRewardEarned(RewardType type)
        {
            if (ServiceLocator.TryGet<RewardHandler>(out var handler))
                handler.HandleReward(type);

            _rewardCallback?.Invoke(true);
            _rewardCallback = null;

            PreloadRewarded();
        }

        private void PreloadRewarded()
        {
            Debug.Log($"[AdManager] Preloading rewarded: {_config.RewardedId}");
            // RewardedAd.Load(_config.RewardedId, request, (ad, error) => { ... });
        }

        public bool IsRewardedReady()
        {
            // return _rewardedAd != null && _rewardedAd.CanShowAd();
            return IsInitialized;
        }
    }
}
