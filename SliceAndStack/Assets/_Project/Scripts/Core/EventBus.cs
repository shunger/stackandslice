using System;
using System.Collections.Generic;

namespace SliceAndStack.Core
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> _events = new();

        public static void Subscribe<T>(Action<T> handler) where T : struct
        {
            var type = typeof(T);
            if (_events.TryGetValue(type, out var existing))
                _events[type] = Delegate.Combine(existing, handler);
            else
                _events[type] = handler;
        }

        public static void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            var type = typeof(T);
            if (_events.TryGetValue(type, out var existing))
            {
                var result = Delegate.Remove(existing, handler);
                if (result == null)
                    _events.Remove(type);
                else
                    _events[type] = result;
            }
        }

        public static void Publish<T>(T eventData) where T : struct
        {
            var type = typeof(T);
            if (_events.TryGetValue(type, out var handler))
                ((Action<T>)handler)?.Invoke(eventData);
        }

        public static void Clear()
        {
            _events.Clear();
        }
    }

    // Game Events
    public struct GameStateChangedEvent
    {
        public GameState PreviousState;
        public GameState NewState;
    }

    public struct ObjectSpawnedEvent
    {
        public UnityEngine.GameObject Object;
    }

    public struct SlicePerformedEvent
    {
        public SliceQuality Quality;
        public UnityEngine.Vector2 SlicePosition;
        public UnityEngine.GameObject LeftHalf;
        public UnityEngine.GameObject RightHalf;
    }

    public enum SliceQuality
    {
        Miss,
        Good,
        Great,
        Perfect
    }

    public struct PieceLandedEvent
    {
        public UnityEngine.GameObject Piece;
        public float ImpactForce;
        public float TowerHeight;
    }

    public struct ScoreChangedEvent
    {
        public int CurrentScore;
        public int PointsAdded;
        public int Combo;
    }

    public struct ComboChangedEvent
    {
        public int Combo;
        public bool OnFire;
    }

    public struct StabilityChangedEvent
    {
        public float Stability;
        public StabilityLevel Level;
    }

    public enum StabilityLevel
    {
        Normal,
        Warning,
        Alert,
        Critical
    }

    public struct TowerCollapsedEvent
    {
        public float FinalHeight;
        public int FinalScore;
    }

    public struct GameOverEvent
    {
        public int FinalScore;
        public float TowerHeight;
        public bool IsNewRecord;
    }

    public struct ReviveEvent { }

    public struct DifficultyChangedEvent
    {
        public float SpawnInterval;
        public float FallSpeed;
        public float SliceZoneSize;
        public float WindStrength;
    }

    public struct NewRecordEvent
    {
        public int Score;
    }

    public struct ThemeChangedEvent
    {
        public string ThemeId;
    }

    public struct AdRewardedEvent
    {
        public RewardType Type;
    }

    public enum RewardType
    {
        Revive,
        DoubleScore
    }
}
