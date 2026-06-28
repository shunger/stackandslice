using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.VFX
{
    public class JuiceManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _sliceParticlesPrefab;
        [SerializeField] private GameObject _landingParticlesPrefab;
        [SerializeField] private GameObject _collapseParticlesPrefab;

        [Header("References")]
        [SerializeField] private SliceTrailEffect _sliceTrail;

        [Header("Flash")]
        [SerializeField] private SpriteRenderer _screenFlash;
        [SerializeField] private float _flashDuration = 0.1f;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            EventBus.Subscribe<SlicePerformedEvent>(OnSlice);
            EventBus.Subscribe<PieceLandedEvent>(OnLanding);
            EventBus.Subscribe<TowerCollapsedEvent>(OnCollapse);
            EventBus.Subscribe<StabilityChangedEvent>(OnStabilityChanged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<SlicePerformedEvent>(OnSlice);
            EventBus.Unsubscribe<PieceLandedEvent>(OnLanding);
            EventBus.Unsubscribe<TowerCollapsedEvent>(OnCollapse);
            EventBus.Unsubscribe<StabilityChangedEvent>(OnStabilityChanged);
            ServiceLocator.Unregister<JuiceManager>();
        }

        private void OnSlice(SlicePerformedEvent evt)
        {
            if (evt.Quality == SliceQuality.Miss) return;

            // Slice trail
            if (_sliceTrail != null)
                _sliceTrail.ShowSliceTrail(evt.SlicePosition, Color.white);

            // Particles
            if (_sliceParticlesPrefab != null)
            {
                int count = evt.Quality switch
                {
                    SliceQuality.Perfect => 40,
                    SliceQuality.Great => 25,
                    _ => 15
                };

                var particles = Instantiate(_sliceParticlesPrefab,
                    (Vector3)evt.SlicePosition, Quaternion.identity);
                var ps = particles.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var emission = ps.emission;
                    var burst = emission.GetBurst(0);
                    burst.count = count;
                    emission.SetBurst(0, burst);
                    ps.Play();
                }
                Destroy(particles, 2f);
            }

            // Slow-mo for perfect
            if (evt.Quality == SliceQuality.Perfect)
            {
                if (ServiceLocator.TryGet<SlowMotionController>(out var slowMo))
                    slowMo.TriggerPerfectSlowMo();

                FlashScreen(new Color(1f, 1f, 1f, 0.3f));

                if (ServiceLocator.TryGet<Haptics.HapticManager>(out var haptic))
                    haptic.PlaySuccess();
            }
            else
            {
                if (ServiceLocator.TryGet<Haptics.HapticManager>(out var haptic))
                    haptic.PlayLight();
            }

            // Audio
            if (ServiceLocator.TryGet<Audio.AudioManager>(out var audio))
                audio.PlaySfx("slice_" + evt.Quality.ToString().ToLower());
        }

        private void OnLanding(PieceLandedEvent evt)
        {
            // Squash and stretch (DOTween would be used here)
            var t = evt.Piece.transform;
            // DOTween: t.DOPunchScale(new Vector3(0.1f, -squashAmount, 0), squashDuration, 1, 0);

            // Screen shake proportional to impact
            if (ServiceLocator.TryGet<ScreenShaker>(out var shaker))
                shaker.ShakeByImpact(evt.ImpactForce);

            // Landing particles
            if (_landingParticlesPrefab != null)
            {
                var particles = Instantiate(_landingParticlesPrefab,
                    evt.Piece.transform.position, Quaternion.identity);
                Destroy(particles, 2f);
            }

            // Haptic
            if (ServiceLocator.TryGet<Haptics.HapticManager>(out var haptic))
            {
                if (evt.ImpactForce > 5f)
                    haptic.PlayMedium();
                else
                    haptic.PlayLight();
            }

            // Audio
            if (ServiceLocator.TryGet<Audio.AudioManager>(out var audio))
                audio.PlaySfx("land");
        }

        private void OnCollapse(TowerCollapsedEvent evt)
        {
            // Slow motion
            if (ServiceLocator.TryGet<SlowMotionController>(out var slowMo))
                slowMo.TriggerCollapseSlowMo();

            // Big screen shake
            if (ServiceLocator.TryGet<ScreenShaker>(out var shaker))
                shaker.Shake(0.2f, 1.0f);

            // Collapse particles
            if (_collapseParticlesPrefab != null)
            {
                var particles = Instantiate(_collapseParticlesPrefab,
                    Vector3.up * 2f, Quaternion.identity);
                Destroy(particles, 3f);
            }

            // Heavy haptic
            if (ServiceLocator.TryGet<Haptics.HapticManager>(out var haptic))
                haptic.PlayHeavy();

            // Audio
            if (ServiceLocator.TryGet<Audio.AudioManager>(out var audio))
            {
                audio.PlaySfx("collapse");
                audio.Music.Duck();
            }
        }

        private void OnStabilityChanged(StabilityChangedEvent evt)
        {
            switch (evt.Level)
            {
                case StabilityLevel.Warning:
                    if (ServiceLocator.TryGet<Audio.AudioManager>(out var audio1))
                        audio1.PlaySfx("creak");
                    if (ServiceLocator.TryGet<Haptics.HapticManager>(out var haptic1))
                        haptic1.PlayLight();
                    break;

                case StabilityLevel.Alert:
                    if (ServiceLocator.TryGet<Audio.AudioManager>(out var audio2))
                        audio2.PlaySfx("warning");
                    if (ServiceLocator.TryGet<Haptics.HapticManager>(out var haptic2))
                        haptic2.PlayWarning();
                    break;

                case StabilityLevel.Critical:
                    if (ServiceLocator.TryGet<Audio.AudioManager>(out var audio3))
                        audio3.PlaySfx("alarm");
                    if (ServiceLocator.TryGet<Haptics.HapticManager>(out var haptic3))
                        haptic3.PlayError();
                    break;
            }
        }

        private void FlashScreen(Color color)
        {
            if (_screenFlash == null) return;
            _screenFlash.color = color;
            _screenFlash.enabled = true;
            // Would use DOTween: _screenFlash.DOFade(0f, _flashDuration).OnComplete(() => _screenFlash.enabled = false);
            Invoke(nameof(HideFlash), _flashDuration);
        }

        private void HideFlash()
        {
            if (_screenFlash != null)
                _screenFlash.enabled = false;
        }
    }
}
