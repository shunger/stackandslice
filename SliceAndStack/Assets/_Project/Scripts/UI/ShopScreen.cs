using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SliceAndStack.Core;
using SliceAndStack.Cosmetics;

namespace SliceAndStack.UI
{
    public class ShopScreen : UIScreen
    {
        [Header("UI")]
        [SerializeField] private Transform _themeListParent;
        [SerializeField] private GameObject _themeItemPrefab;
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _activeThemeLabel;

        private readonly List<GameObject> _spawnedItems = new();

        protected override void Awake()
        {
            base.Awake();
            _backButton?.onClick.AddListener(OnBackClicked);
        }

        public override void Show()
        {
            base.Show();
            PopulateThemes();
        }

        public override void Hide()
        {
            ClearItems();
            base.Hide();
        }

        private void PopulateThemes()
        {
            ClearItems();

            if (!ServiceLocator.TryGet<CosmeticManager>(out var cosmetics)) return;

            var themes = cosmetics.GetAllThemes();
            foreach (var theme in themes)
            {
                if (theme == null || _themeItemPrefab == null || _themeListParent == null) continue;

                var item = Instantiate(_themeItemPrefab, _themeListParent);
                _spawnedItems.Add(item);

                bool unlocked = cosmetics.IsThemeUnlocked(theme.themeId);
                bool isActive = cosmetics.ActiveTheme != null &&
                                cosmetics.ActiveTheme.themeId == theme.themeId;

                // Name
                var nameText = item.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
                if (nameText != null)
                    nameText.text = theme.displayName;

                // Icon
                var iconImg = item.transform.Find("Icon")?.GetComponent<Image>();
                if (iconImg != null && theme.icon != null)
                    iconImg.sprite = theme.icon;

                // Status
                var statusText = item.transform.Find("Status")?.GetComponent<TextMeshProUGUI>();
                if (statusText != null)
                {
                    if (isActive) statusText.text = "EQUIPPED";
                    else if (unlocked) statusText.text = "UNLOCKED";
                    else
                    {
                        float progress = cosmetics.GetUnlockProgress(theme.themeId);
                        statusText.text = $"{Mathf.RoundToInt(progress * 100)}%";
                    }
                }

                // Button
                var button = item.GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = unlocked && !isActive;
                    string themeId = theme.themeId;
                    button.onClick.AddListener(() => OnThemeSelected(themeId));
                }
            }

            if (_activeThemeLabel != null && cosmetics.ActiveTheme != null)
                _activeThemeLabel.text = $"Active: {cosmetics.ActiveTheme.displayName}";
        }

        private void OnThemeSelected(string themeId)
        {
            if (ServiceLocator.TryGet<CosmeticManager>(out var cosmetics))
            {
                cosmetics.SetTheme(themeId);
                PopulateThemes();
            }

            if (ServiceLocator.TryGet<Audio.AudioManager>(out var audio))
                audio.PlaySfx("ui_click");
        }

        private void ClearItems()
        {
            foreach (var item in _spawnedItems)
                Destroy(item);
            _spawnedItems.Clear();
        }

        private void OnBackClicked()
        {
            if (ServiceLocator.TryGet<UIManager>(out var ui))
                ui.Back();
        }
    }
}
