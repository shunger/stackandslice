using System.Collections.Generic;
using UnityEngine;
using SliceAndStack.Core;
using SliceAndStack.Difficulty;

namespace SliceAndStack.Spawning
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private List<SliceableData> _sliceableDataList;
        [SerializeField] private float _spawnYOffset = 1.5f;
        [SerializeField] private float _horizontalSpawnRange = 0.3f;

        private float _spawnTimer;
        private float _currentSpawnInterval = 3.0f;
        private float _currentFallSpeed = 2.0f;
        private float _sliceZoneBottom;
        private float _sliceZoneTop;
        private Camera _mainCamera;
        private bool _isSpawning;

        private void Awake()
        {
            ServiceLocator.Register(this);
            _mainCamera = Camera.main;
            UpdateSliceZoneBounds(Utils.Constants.SLICE_ZONE_DEFAULT_SIZE);
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<DifficultyChangedEvent>(OnDifficultyChanged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<DifficultyChangedEvent>(OnDifficultyChanged);
            ServiceLocator.Unregister<SpawnManager>();
        }

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            _isSpawning = evt.NewState == GameState.Playing;
        }

        private void OnDifficultyChanged(DifficultyChangedEvent evt)
        {
            _currentSpawnInterval = evt.SpawnInterval;
            _currentFallSpeed = evt.FallSpeed;
            UpdateSliceZoneBounds(evt.SliceZoneSize);
        }

        private void UpdateSliceZoneBounds(float sliceZoneSize)
        {
            float zoneCenter = Utils.MathUtils.ScreenPercentToWorldY(
                Utils.Constants.SLICE_ZONE_Y_POSITION, _mainCamera);
            float halfHeight = Utils.MathUtils.ScreenPercentToWorldY(
                Utils.Constants.SLICE_ZONE_Y_POSITION + sliceZoneSize / 2f, _mainCamera) - zoneCenter;

            _sliceZoneBottom = zoneCenter - halfHeight;
            _sliceZoneTop = zoneCenter + halfHeight;
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
                _isSpawning = true;

            if (!_isSpawning) return;

            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= _currentSpawnInterval)
            {
                SpawnObject();
                _spawnTimer = 0f;
            }
        }

        private void SpawnObject()
        {
            if (_sliceableDataList == null || _sliceableDataList.Count == 0)
            {
                Debug.LogWarning("[SpawnManager] No sliceable data in list");
                return;
            }

            var pool = ServiceLocator.Get<ObjectPool>();
            if (pool == null)
            {
                Debug.LogWarning("[SpawnManager] ObjectPool not found");
                return;
            }

            var data = _sliceableDataList[Random.Range(0, _sliceableDataList.Count)];

            var obj = pool.GetSliceable();
            var sliceable = obj.GetComponent<SliceableObject>();

            // Position at top of screen with random horizontal offset
            float screenTop = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0f)).y;
            float screenLeft = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f - _horizontalSpawnRange, 0f, 0f)).x;
            float screenRight = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f + _horizontalSpawnRange, 0f, 0f)).x;

            float spawnX = Random.Range(screenLeft, screenRight);
            float spawnY = screenTop + _spawnYOffset;

            obj.transform.position = new Vector3(spawnX, spawnY, 0f);
            obj.transform.rotation = Quaternion.identity;

            sliceable.Initialize(data, _currentFallSpeed, _sliceZoneBottom, _sliceZoneTop);

            EventBus.Publish(new ObjectSpawnedEvent { Object = obj });
        }

        public void Reset()
        {
            _spawnTimer = 0f;
            _currentSpawnInterval = 3.0f;
            _currentFallSpeed = 2.0f;
            UpdateSliceZoneBounds(Utils.Constants.SLICE_ZONE_DEFAULT_SIZE);

            if (ServiceLocator.TryGet<ObjectPool>(out var pool))
                pool.ReturnAll();
        }

        public float SliceZoneBottom => _sliceZoneBottom;
        public float SliceZoneTop => _sliceZoneTop;
    }
}
