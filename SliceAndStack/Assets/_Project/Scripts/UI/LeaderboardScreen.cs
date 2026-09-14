using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SliceAndStack.Core;
using SliceAndStack.Utils;

namespace SliceAndStack.UI
{
    public class LeaderboardScreen : UIScreen
    {
        [Header("UI")]
        [SerializeField] private Transform _entryListParent;
        [SerializeField] private GameObject _entryItemPrefab;
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _noEntriesText;

        private readonly List<GameObject> _spawnedItems = new();

        protected override void Awake()
        {
            base.Awake();
            _backButton?.onClick.AddListener(OnBackClicked);
        }

        public override void Show()
        {
            base.Show();
            PopulateLeaderboard();
        }

        public override void Hide()
        {
            ClearItems();
            base.Hide();
        }

        private void PopulateLeaderboard()
        {
            ClearItems();

            if (!ServiceLocator.TryGet<Persistence.SaveManager>(out var save)) return;

            var entries = save.Data.leaderboard;

            if (_noEntriesText != null)
                _noEntriesText.gameObject.SetActive(entries.Count == 0);

            for (int i = 0; i < entries.Count; i++)
            {
                if (_entryItemPrefab == null || _entryListParent == null) continue;

                var item = Instantiate(_entryItemPrefab, _entryListParent);
                _spawnedItems.Add(item);

                var rankText = item.transform.Find("Rank")?.GetComponent<TextMeshProUGUI>();
                if (rankText != null)
                    rankText.text = $"#{i + 1}";

                var scoreText = item.transform.Find("Score")?.GetComponent<TextMeshProUGUI>();
                if (scoreText != null)
                    scoreText.text = entries[i].score.FormatScore();

                var heightText = item.transform.Find("Height")?.GetComponent<TextMeshProUGUI>();
                if (heightText != null)
                    heightText.text = entries[i].towerHeight.FormatHeight();

                var dateText = item.transform.Find("Date")?.GetComponent<TextMeshProUGUI>();
                if (dateText != null)
                {
                    var date = System.DateTimeOffset.FromUnixTimeSeconds(entries[i].timestamp).LocalDateTime;
                    dateText.text = date.ToString("MMM dd");
                }
            }
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
