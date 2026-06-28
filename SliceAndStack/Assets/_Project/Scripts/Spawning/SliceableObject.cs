using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Spawning
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SliceableObject : MonoBehaviour
    {
        public SliceableData Data { get; private set; }
        public bool IsInSliceZone { get; private set; }
        public bool HasBeenSliced { get; private set; }

        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private float _fallSpeed;
        private float _sliceZoneTop;
        private float _sliceZoneBottom;

        public void Initialize(SliceableData data, float fallSpeed, float sliceZoneBottom, float sliceZoneTop)
        {
            Data = data;
            _fallSpeed = fallSpeed;
            _sliceZoneBottom = sliceZoneBottom;
            _sliceZoneTop = sliceZoneTop;
            HasBeenSliced = false;
            IsInSliceZone = false;

            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;

            _spriteRenderer.sprite = data.wholeSprite;

            if (data.wholeSprite != null)
            {
                var col = GetComponent<BoxCollider2D>();
                if (col == null) col = gameObject.AddComponent<BoxCollider2D>();
                col.size = data.colliderSize;
                col.isTrigger = true;
            }
        }

        private void Update()
        {
            if (HasBeenSliced) return;

            transform.Translate(Vector3.down * _fallSpeed * Time.deltaTime);

            float y = transform.position.y;
            IsInSliceZone = y >= _sliceZoneBottom && y <= _sliceZoneTop;

            if (y < _sliceZoneBottom - Utils.Constants.OFF_SCREEN_MARGIN)
            {
                OnMissed();
            }
        }

        public float GetSliceZonePosition()
        {
            if (!IsInSliceZone) return -1f;
            float zoneCenter = (_sliceZoneTop + _sliceZoneBottom) / 2f;
            float zoneHalfHeight = (_sliceZoneTop - _sliceZoneBottom) / 2f;
            if (zoneHalfHeight <= 0f) return 0f;
            return 1f - Mathf.Abs(transform.position.y - zoneCenter) / zoneHalfHeight;
        }

        public SliceQuality GetSliceQuality()
        {
            if (!IsInSliceZone) return SliceQuality.Miss;

            float normalizedPos = GetSliceZonePosition();

            if (normalizedPos >= 0.9f) return SliceQuality.Perfect;
            if (normalizedPos >= 0.7f) return SliceQuality.Great;
            return SliceQuality.Good;
        }

        public void MarkSliced()
        {
            HasBeenSliced = true;
            _spriteRenderer.enabled = false;
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }

        private void OnMissed()
        {
            if (ServiceLocator.TryGet<Scoring.ComboTracker>(out var combo))
                combo.ResetCombo();

            ReturnToPool();
        }

        public void ReturnToPool()
        {
            if (ServiceLocator.TryGet<ObjectPool>(out var pool))
                pool.Return(gameObject);
            else
                Destroy(gameObject);
        }

        public void ResetState()
        {
            HasBeenSliced = false;
            IsInSliceZone = false;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null) _spriteRenderer.enabled = true;
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = true;
        }
    }
}
