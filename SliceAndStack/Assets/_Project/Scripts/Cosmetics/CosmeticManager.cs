using System.Collections.Generic;
using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Cosmetics
{
    public class CosmeticManager : MonoBehaviour
    {
        [SerializeField] private List<ThemeData> _allThemes;
        [SerializeField] private List<UnlockCondition> _unlockConditions;

        public ThemeData ActiveTheme { get; private set; }

        private Dictionary<string, ThemeData> _themeLookup;

        private void Awake()
        {
            ServiceLocator.Register(this);
            BuildLookup();
        }

        private void Start()
        {
            // Load active theme from save
            if (ServiceLocator.TryGet<Persistence.SaveManager>(out var save))
            {
                SetTheme(save.Data.activeThemeId);
            }
            else if (_allThemes.Count > 0)
            {
                ActiveTheme = _allThemes[0];
            }
        }

        private void BuildLookup()
        {
            _themeLookup = new Dictionary<string, ThemeData>();
            foreach (var theme in _allThemes)
            {
                if (theme != null && !string.IsNullOrEmpty(theme.themeId))
                    _themeLookup[theme.themeId] = theme;
            }
        }

        public void SetTheme(string themeId)
        {
            if (_themeLookup.TryGetValue(themeId, out var theme))
            {
                ActiveTheme = theme;

                if (ServiceLocator.TryGet<Persistence.SaveManager>(out var save))
                {
                    save.Data.activeThemeId = themeId;
                    save.Save();
                }

                EventBus.Publish(new ThemeChangedEvent { ThemeId = themeId });
            }
        }

        public void CheckUnlocks()
        {
            if (!ServiceLocator.TryGet<Persistence.SaveManager>(out var save)) return;

            foreach (var condition in _unlockConditions)
            {
                if (condition == null) continue;
                if (save.Data.IsThemeUnlocked(condition.themeId)) continue;

                if (condition.IsMet(save.Data))
                {
                    save.Data.UnlockTheme(condition.themeId);
                    Debug.Log($"[CosmeticManager] Theme unlocked: {condition.themeId}");
                }
            }

            save.Save();
        }

        public List<ThemeData> GetAllThemes() => _allThemes;

        public bool IsThemeUnlocked(string themeId)
        {
            if (!ServiceLocator.TryGet<Persistence.SaveManager>(out var save)) return false;
            return save.Data.IsThemeUnlocked(themeId);
        }

        public float GetUnlockProgress(string themeId)
        {
            if (!ServiceLocator.TryGet<Persistence.SaveManager>(out var save)) return 0f;

            foreach (var condition in _unlockConditions)
            {
                if (condition != null && condition.themeId == themeId)
                    return condition.GetProgress(save.Data);
            }

            return 0f;
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<CosmeticManager>();
        }
    }
}
