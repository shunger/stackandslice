using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Slicing
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SliceZoneVisual : MonoBehaviour
    {
        [SerializeField] private SliceConfig _config;

        private SpriteRenderer _renderer;
        private Camera _mainCamera;
        private float _targetAlpha = 0f;
        private float _currentAlpha;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<DifficultyChangedEvent>(OnDifficultyChanged);
            UpdateVisual(Utils.Constants.SLICE_ZONE_DEFAULT_SIZE);
            _renderer.color = _config != null ? _config.sliceZoneColor : new Color(1f, 1f, 1f, 0.15f);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<DifficultyChangedEvent>(OnDifficultyChanged);
        }

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            _targetAlpha = evt.NewState == GameState.Playing ? 0.15f : 0f;
        }

        private void OnDifficultyChanged(DifficultyChangedEvent evt)
        {
            UpdateVisual(evt.SliceZoneSize);
        }

        private void UpdateVisual(float sliceZoneSize)
        {
            if (_mainCamera == null) return;

            float zoneCenter = Utils.MathUtils.ScreenPercentToWorldY(
                Utils.Constants.SLICE_ZONE_Y_POSITION, _mainCamera);
            float zoneTop = Utils.MathUtils.ScreenPercentToWorldY(
                Utils.Constants.SLICE_ZONE_Y_POSITION + sliceZoneSize / 2f, _mainCamera);
            float zoneHeight = (zoneTop - zoneCenter) * 2f;

            float screenWidth = _mainCamera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x -
                               _mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;

            transform.position = new Vector3(0f, zoneCenter, 0f);
            transform.localScale = new Vector3(screenWidth, zoneHeight, 1f);
        }

        private void Update()
        {
            _currentAlpha = Mathf.Lerp(_currentAlpha, _targetAlpha, Time.deltaTime * 5f);
            if (_renderer != null)
            {
                var c = _renderer.color;
                c.a = _currentAlpha;
                _renderer.color = c;
            }
        }
    }
}
