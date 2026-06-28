using System.Collections;
using UnityEngine;

namespace SliceAndStack.Audio
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private float _crossfadeDuration = 1.5f;
        [SerializeField] private float _duckVolume = 0.3f;
        [SerializeField] private float _duckDuration = 0.5f;

        private AudioSource _sourceA;
        private AudioSource _sourceB;
        private AudioSource _activeSource;
        private float _masterVolume = 1f;
        private bool _isDucked;
        private Coroutine _crossfadeCoroutine;
        private Coroutine _duckCoroutine;

        private void Awake()
        {
            _sourceA = gameObject.AddComponent<AudioSource>();
            _sourceB = gameObject.AddComponent<AudioSource>();

            _sourceA.loop = true;
            _sourceB.loop = true;
            _sourceA.playOnAwake = false;
            _sourceB.playOnAwake = false;

            _activeSource = _sourceA;
        }

        public void Play(AudioClip clip, float volume = 1f)
        {
            if (_activeSource.clip == clip && _activeSource.isPlaying) return;

            if (_activeSource.isPlaying)
            {
                Crossfade(clip, volume);
            }
            else
            {
                _activeSource.clip = clip;
                _activeSource.volume = volume * _masterVolume;
                _activeSource.Play();
            }
        }

        public void Stop()
        {
            if (_crossfadeCoroutine != null)
                StopCoroutine(_crossfadeCoroutine);

            _sourceA.Stop();
            _sourceB.Stop();
        }

        public void SetVolume(float volume)
        {
            _masterVolume = volume;
            if (!_isDucked)
                _activeSource.volume = _masterVolume;
        }

        public void Duck()
        {
            if (_duckCoroutine != null)
                StopCoroutine(_duckCoroutine);
            _duckCoroutine = StartCoroutine(DuckRoutine(true));
        }

        public void Unduck()
        {
            if (_duckCoroutine != null)
                StopCoroutine(_duckCoroutine);
            _duckCoroutine = StartCoroutine(DuckRoutine(false));
        }

        private void Crossfade(AudioClip newClip, float volume)
        {
            if (_crossfadeCoroutine != null)
                StopCoroutine(_crossfadeCoroutine);

            var incoming = _activeSource == _sourceA ? _sourceB : _sourceA;
            var outgoing = _activeSource;

            incoming.clip = newClip;
            incoming.volume = 0f;
            incoming.Play();

            _activeSource = incoming;
            _crossfadeCoroutine = StartCoroutine(CrossfadeRoutine(outgoing, incoming, volume));
        }

        private IEnumerator CrossfadeRoutine(AudioSource outgoing, AudioSource incoming, float targetVolume)
        {
            float elapsed = 0f;
            float outStartVol = outgoing.volume;

            while (elapsed < _crossfadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / _crossfadeDuration;

                outgoing.volume = Mathf.Lerp(outStartVol, 0f, t);
                incoming.volume = Mathf.Lerp(0f, targetVolume * _masterVolume, t);

                yield return null;
            }

            outgoing.Stop();
            outgoing.volume = 0f;
            incoming.volume = targetVolume * _masterVolume;
        }

        private IEnumerator DuckRoutine(bool duck)
        {
            _isDucked = duck;
            float startVol = _activeSource.volume;
            float targetVol = duck ? _duckVolume * _masterVolume : _masterVolume;
            float elapsed = 0f;

            while (elapsed < _duckDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _activeSource.volume = Mathf.Lerp(startVol, targetVol, elapsed / _duckDuration);
                yield return null;
            }

            _activeSource.volume = targetVol;
        }
    }
}
