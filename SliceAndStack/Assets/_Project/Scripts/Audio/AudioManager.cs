using System.Collections.Generic;
using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private SoundLibrary _soundLibrary;
        [SerializeField] private int _sfxPoolSize = 16;
        [SerializeField] private UnityEngine.Audio.AudioMixer _mixer;

        public MusicPlayer Music { get; private set; }

        private readonly Queue<AudioSource> _sfxPool = new();
        private readonly List<AudioSource> _activeSfx = new();
        private float _sfxVolume = 1f;
        private float _musicVolume = 1f;
        private bool _sfxMuted;
        private bool _musicMuted;

        private void Awake()
        {
            ServiceLocator.Register(this);

            Music = GetComponent<MusicPlayer>();
            if (Music == null) Music = gameObject.AddComponent<MusicPlayer>();

            InitSfxPool();
        }

        private void InitSfxPool()
        {
            for (int i = 0; i < _sfxPoolSize; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = false;
                _sfxPool.Enqueue(source);
            }
        }

        public void PlaySfx(string soundId)
        {
            if (_sfxMuted) return;
            if (_soundLibrary == null) return;

            var entry = _soundLibrary.GetSound(soundId);
            if (entry == null || entry.clip == null) return;

            var source = GetPooledSource();
            if (source == null) return;

            source.clip = entry.clip;
            source.volume = entry.volume * _sfxVolume;
            source.pitch = Random.Range(entry.pitchMin, entry.pitchMax);
            source.loop = entry.loop;
            source.Play();

            if (!entry.loop)
                _activeSfx.Add(source);
        }

        public void PlaySfxAtPosition(string soundId, Vector3 position)
        {
            if (_sfxMuted || _soundLibrary == null) return;

            var entry = _soundLibrary.GetSound(soundId);
            if (entry == null || entry.clip == null) return;

            AudioSource.PlayClipAtPoint(entry.clip, position, entry.volume * _sfxVolume);
        }

        public void PlayMusic(string musicId)
        {
            if (_soundLibrary == null) return;

            var entry = _soundLibrary.GetMusic(musicId);
            if (entry == null || entry.clip == null) return;

            Music.Play(entry.clip, _musicMuted ? 0f : entry.volume * _musicVolume);
        }

        public void StopMusic()
        {
            Music.Stop();
        }

        public void SetSfxVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            Music.SetVolume(_musicMuted ? 0f : _musicVolume);
        }

        public void SetSfxMuted(bool muted)
        {
            _sfxMuted = muted;
        }

        public void SetMusicMuted(bool muted)
        {
            _musicMuted = muted;
            Music.SetVolume(muted ? 0f : _musicVolume);
        }

        private AudioSource GetPooledSource()
        {
            // Return finished sources to pool
            for (int i = _activeSfx.Count - 1; i >= 0; i--)
            {
                if (!_activeSfx[i].isPlaying)
                {
                    _sfxPool.Enqueue(_activeSfx[i]);
                    _activeSfx.RemoveAt(i);
                }
            }

            if (_sfxPool.Count > 0)
                return _sfxPool.Dequeue();

            // Steal oldest active source
            if (_activeSfx.Count > 0)
            {
                var oldest = _activeSfx[0];
                _activeSfx.RemoveAt(0);
                oldest.Stop();
                return oldest;
            }

            return null;
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<AudioManager>();
        }
    }
}
