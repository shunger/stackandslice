using UnityEngine;
using SliceAndStack.Core;
using SliceAndStack.Slicing;

namespace SliceAndStack.Input
{
    public class InputManager : MonoBehaviour
    {
        private TapDetector _tapDetector;
        private SwipeDetector _swipeDetector;
        private bool _isActive;

        private void Awake()
        {
            ServiceLocator.Register(this);
            _tapDetector = new TapDetector();
            _swipeDetector = new SwipeDetector();

            _tapDetector.OnTap += HandleTap;
        }

        private void Start()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            _tapDetector.OnTap -= HandleTap;
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            ServiceLocator.Unregister<InputManager>();
        }

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            _isActive = evt.NewState == GameState.Playing;
        }

        private void Update()
        {
            if (!_isActive) return;

            // Touch input
            if (UnityEngine.Input.touchCount > 0)
            {
                var touch = UnityEngine.Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        _tapDetector.ProcessTouchDown(touch.position);
                        _swipeDetector.ProcessTouchDown(touch.position);
                        break;
                    case TouchPhase.Ended:
                        _tapDetector.ProcessTouchUp(touch.position);
                        _swipeDetector.ProcessTouchUp(touch.position);
                        break;
                    case TouchPhase.Canceled:
                        _tapDetector.ProcessTouchCancel();
                        _swipeDetector.ProcessTouchCancel();
                        break;
                }
            }

            // Mouse input (for editor testing)
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                _tapDetector.ProcessTouchDown(UnityEngine.Input.mousePosition);
                _swipeDetector.ProcessTouchDown(UnityEngine.Input.mousePosition);
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                _tapDetector.ProcessTouchUp(UnityEngine.Input.mousePosition);
                _swipeDetector.ProcessTouchUp(UnityEngine.Input.mousePosition);
            }
        }

        private void HandleTap(Vector2 screenPosition)
        {
            if (!ServiceLocator.TryGet<SliceDetector>(out var detector)) return;

            var result = detector.TrySlice(screenPosition);
            if (result.IsValid)
            {
                if (ServiceLocator.TryGet<SliceExecutor>(out var executor))
                    executor.Execute(result);
            }
            else if (result.Quality == SliceQuality.Miss)
            {
                // Missed tap - could trigger miss feedback
                EventBus.Publish(new SlicePerformedEvent
                {
                    Quality = SliceQuality.Miss,
                    SlicePosition = screenPosition
                });
            }
        }
    }
}
