using System;
using System.Collections.Generic;
using UnityEngine;

namespace SliceAndStack.Audio
{
    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "SliceAndStack/Audio/Sound Library")]
    public class SoundLibrary : ScriptableObject
    {
        [Serializable]
        public class SoundEntry
        {
            public string id;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            [Range(0.5f, 2f)] public float pitchMin = 0.95f;
            [Range(0.5f, 2f)] public float pitchMax = 1.05f;
            public bool loop;
        }

        public List<SoundEntry> sounds = new();
        public List<SoundEntry> music = new();

        private Dictionary<string, SoundEntry> _soundLookup;
        private Dictionary<string, SoundEntry> _musicLookup;

        public SoundEntry GetSound(string id)
        {
            if (_soundLookup == null) BuildLookup();
            _soundLookup.TryGetValue(id, out var entry);
            return entry;
        }

        public SoundEntry GetMusic(string id)
        {
            if (_musicLookup == null) BuildLookup();
            _musicLookup.TryGetValue(id, out var entry);
            return entry;
        }

        private void BuildLookup()
        {
            _soundLookup = new Dictionary<string, SoundEntry>();
            foreach (var s in sounds)
            {
                if (!string.IsNullOrEmpty(s.id))
                    _soundLookup[s.id] = s;
            }

            _musicLookup = new Dictionary<string, SoundEntry>();
            foreach (var m in music)
            {
                if (!string.IsNullOrEmpty(m.id))
                    _musicLookup[m.id] = m;
            }
        }

        private void OnEnable()
        {
            _soundLookup = null;
            _musicLookup = null;
        }
    }
}
