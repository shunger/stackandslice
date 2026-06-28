using System.Collections;
using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.VFX
{
    public class SlowMotionController : MonoBehaviour
    {
        private Coroutine _slowMoCoroutine;
        private float _defaultFixedDeltaTime;

        private void Awake()
        {
            ServiceLocator.Register(this);
            _defaultFixedDeltaTime = Time.fixedDeltaTime;
        }

        public void TriggerSlowMotion(float timeScale, float duration)
        {
            if (_slowMoCoroutine != null)
                StopCoroutine(_slowMoCoroutine);

            _slowMoCoroutine = StartCoroutine(SlowMotionRoutine(timeScale, duration));
        }

        public void TriggerPerfectSlowMo()
        {
            TriggerSlowMotion(
                Utils.Constants.SLOW_MO_PERFECT_TIMESCALE,
                Utils.Constants.SLOW_MO_PERFECT_DURATION);
        }

        public void TriggerCollapseSlowMo()
        {
            TriggerSlowMotion(
                Utils.Constants.SLOW_MO_COLLAPSE_TIMESCALE,
                Utils.Constants.SLOW_MO_COLLAPSE_DURATION);
        }

        private IEnumerator SlowMotionRoutine(float targetTimeScale, float duration)
        {
            // Ease into slow-mo
            float originalTimeScale = Time.timeScale;
            float easeInDuration = 0.05f;
            float elapsed = 0f;

            while (elapsed < easeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / easeInDuration;
                Time.timeScale = Mathf.Lerp(originalTimeScale, targetTimeScale, t);
                Time.fixedDeltaTime = _defaultFixedDeltaTime * Time.timeScale;
                yield return null;
            }

            Time.timeScale = targetTimeScale;
            Time.fixedDeltaTime = _defaultFixedDeltaTime * targetTimeScale;

            // Hold slow-mo (using unscaled time)
            yield return new WaitForSecondsRealtime(duration);

            // Ease out
            elapsed = 0f;
            float easeOutDuration = 0.15f;

            while (elapsed < easeOutDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / easeOutDuration;
                Time.timeScale = Mathf.Lerp(targetTimeScale, 1f, t);
                Time.fixedDeltaTime = _defaultFixedDeltaTime * Time.timeScale;
                yield return null;
            }

            Time.timeScale = 1f;
            Time.fixedDeltaTime = _defaultFixedDeltaTime;
            _slowMoCoroutine = null;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = _defaultFixedDeltaTime;
            ServiceLocator.Unregister<SlowMotionController>();
        }
    }
}
