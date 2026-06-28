using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.VFX
{
    public class SliceTrailEffect : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _trailDuration = 0.3f;
        [SerializeField] private float _trailWidth = 0.1f;
        [SerializeField] private Color _trailColor = Color.white;

        private float _timer;
        private bool _isActive;

        private void Awake()
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();
                if (_lineRenderer == null)
                    _lineRenderer = gameObject.AddComponent<LineRenderer>();
            }

            _lineRenderer.startWidth = _trailWidth;
            _lineRenderer.endWidth = _trailWidth * 0.5f;
            _lineRenderer.startColor = _trailColor;
            _lineRenderer.endColor = new Color(_trailColor.r, _trailColor.g, _trailColor.b, 0f);
            _lineRenderer.positionCount = 0;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.enabled = false;
        }

        public void ShowSliceTrail(Vector2 position, Color color)
        {
            _trailColor = color;
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = new Color(color.r, color.g, color.b, 0f);

            // Create horizontal slice line
            float halfWidth = 1.5f;
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, new Vector3(position.x - halfWidth, position.y, 0f));
            _lineRenderer.SetPosition(1, new Vector3(position.x + halfWidth, position.y, 0f));

            _lineRenderer.enabled = true;
            _isActive = true;
            _timer = 0f;
        }

        private void Update()
        {
            if (!_isActive) return;

            _timer += Time.unscaledDeltaTime;
            float t = _timer / _trailDuration;

            if (t >= 1f)
            {
                _lineRenderer.enabled = false;
                _isActive = false;
                return;
            }

            float alpha = 1f - t;
            _lineRenderer.startColor = new Color(_trailColor.r, _trailColor.g, _trailColor.b, alpha);
            _lineRenderer.endColor = new Color(_trailColor.r, _trailColor.g, _trailColor.b, 0f);
            _lineRenderer.startWidth = _trailWidth * (1f - t * 0.5f);
        }
    }
}
