using System.Collections;
using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.VFX
{
    public class ScreenShaker : MonoBehaviour
    {
        private Vector3 _originalPosition;
        private Coroutine _shakeCoroutine;
        private Camera _camera;
        private float _lastShakeTime;
        private const float SHAKE_COOLDOWN = 0.15f;

        private void Awake()
        {
            ServiceLocator.Register(this);
            _camera = Camera.main;
            if (_camera != null)
                _originalPosition = _camera.transform.localPosition;
        }

        public void Shake(float intensity, float duration)
        {
            if (_camera == null) return;

            if (_shakeCoroutine != null)
                StopCoroutine(_shakeCoroutine);

            _lastShakeTime = Time.unscaledTime;
            _shakeCoroutine = StartCoroutine(ShakeRoutine(intensity, duration));
        }

        public void ShakeByImpact(float impactForce)
        {
            // Skip if another impact shake happened very recently
            if (Time.unscaledTime - _lastShakeTime < SHAKE_COOLDOWN) return;

            float intensity = Mathf.Clamp(impactForce * 0.005f, 0.005f, 0.04f);
            float duration = Mathf.Clamp(impactForce * 0.01f, 0.03f, 0.15f);
            Shake(intensity, duration);
        }

        private IEnumerator ShakeRoutine(float intensity, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = 1f - (elapsed / duration);
                float currentIntensity = intensity * t;

                float x = Random.Range(-1f, 1f) * currentIntensity;
                float y = Random.Range(-1f, 1f) * currentIntensity;

                _camera.transform.localPosition = _originalPosition + new Vector3(x, y, 0f);

                yield return null;
            }

            _camera.transform.localPosition = _originalPosition;
            _shakeCoroutine = null;
        }

        private void OnDestroy()
        {
            if (_camera != null)
                _camera.transform.localPosition = _originalPosition;
            ServiceLocator.Unregister<ScreenShaker>();
        }
    }
}
