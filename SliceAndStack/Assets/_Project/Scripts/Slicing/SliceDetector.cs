using UnityEngine;
using SliceAndStack.Core;
using SliceAndStack.Spawning;

namespace SliceAndStack.Slicing
{
    public class SliceDetector : MonoBehaviour
    {
        [SerializeField] private SliceConfig _config;

        private Camera _mainCamera;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            ServiceLocator.Unregister<SliceDetector>();
        }

        private bool _isActive;

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            _isActive = evt.NewState == GameState.Playing;
        }

        public SliceResult TrySlice(Vector2 screenPosition)
        {
            if (!_isActive) return SliceResult.None;

            Vector2 worldPos = _mainCamera.ScreenToWorldPoint(screenPosition);

            // Find the closest sliceable in the slice zone
            SliceableObject bestTarget = null;
            float bestDistance = float.MaxValue;

            foreach (var sliceable in FindObjectsByType<SliceableObject>(FindObjectsSortMode.None))
            {
                if (sliceable.HasBeenSliced || !sliceable.gameObject.activeInHierarchy)
                    continue;

                if (sliceable.IsInSliceZone)
                {
                    float dist = Vector2.Distance(worldPos, sliceable.transform.position);
                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        bestTarget = sliceable;
                    }
                }
            }

            if (bestTarget == null)
            {
                // Check if any sliceable exists but was outside the zone
                bool anyActive = false;
                foreach (var s in FindObjectsByType<SliceableObject>(FindObjectsSortMode.None))
                {
                    if (!s.HasBeenSliced && s.gameObject.activeInHierarchy)
                    {
                        anyActive = true;
                        break;
                    }
                }

                if (anyActive)
                {
                    return new SliceResult
                    {
                        Quality = SliceQuality.Miss,
                        Position = worldPos,
                        Target = null
                    };
                }

                return SliceResult.None;
            }

            var quality = bestTarget.GetSliceQuality();
            return new SliceResult
            {
                Quality = quality,
                Position = bestTarget.transform.position,
                Target = bestTarget,
                NormalizedZonePosition = bestTarget.GetSliceZonePosition()
            };
        }
    }

    public struct SliceResult
    {
        public SliceQuality Quality;
        public Vector2 Position;
        public SliceableObject Target;
        public float NormalizedZonePosition;

        public bool IsValid => Target != null;

        public static SliceResult None => new SliceResult
        {
            Quality = SliceQuality.Miss,
            Target = null
        };
    }
}
