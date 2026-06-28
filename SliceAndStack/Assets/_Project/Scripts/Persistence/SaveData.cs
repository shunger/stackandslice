using System;
using System.Collections.Generic;

namespace SliceAndStack.Persistence
{
    [Serializable]
    public class SaveData
    {
        public int highScore;
        public float highestTower;
        public int totalGamesPlayed;
        public int totalSlices;
        public int totalPerfects;
        public string activeThemeId = "default";
        public List<string> unlockedThemes = new() { "default" };
        public List<LeaderboardEntry> leaderboard = new();
        public SettingsData settings = new();

        public bool TrySetHighScore(int score)
        {
            if (score <= highScore) return false;
            highScore = score;
            return true;
        }

        public bool TrySetHighestTower(float height)
        {
            if (height <= highestTower) return false;
            highestTower = height;
            return true;
        }

        public void AddLeaderboardEntry(int score, float towerHeight)
        {
            leaderboard.Add(new LeaderboardEntry
            {
                score = score,
                towerHeight = towerHeight,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });

            leaderboard.Sort((a, b) => b.score.CompareTo(a.score));

            if (leaderboard.Count > Utils.Constants.LEADERBOARD_MAX_ENTRIES)
                leaderboard.RemoveRange(Utils.Constants.LEADERBOARD_MAX_ENTRIES,
                    leaderboard.Count - Utils.Constants.LEADERBOARD_MAX_ENTRIES);
        }

        public bool IsThemeUnlocked(string themeId)
        {
            return unlockedThemes.Contains(themeId);
        }

        public void UnlockTheme(string themeId)
        {
            if (!unlockedThemes.Contains(themeId))
                unlockedThemes.Add(themeId);
        }
    }

    [Serializable]
    public class LeaderboardEntry
    {
        public int score;
        public float towerHeight;
        public long timestamp;
    }

    [Serializable]
    public class SettingsData
    {
        public float sfxVolume = 1f;
        public float musicVolume = 1f;
        public bool hapticsEnabled = true;
        public bool sfxMuted;
        public bool musicMuted;
    }
}
