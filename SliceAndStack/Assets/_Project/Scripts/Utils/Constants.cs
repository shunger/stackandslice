namespace SliceAndStack.Utils
{
    public static class Constants
    {
        // Layers
        public const string LAYER_SLICEABLE = "Sliceable";
        public const string LAYER_STACKED = "Stacked";
        public const string LAYER_TOWER_BASE = "TowerBase";

        // Tags
        public const string TAG_SLICEABLE = "Sliceable";
        public const string TAG_TOWER_BASE = "TowerBase";
        public const string TAG_SLICE_ZONE = "SliceZone";

        // Sorting Layers
        public const string SORT_BACKGROUND = "Background";
        public const string SORT_TOWER = "Tower";
        public const string SORT_SLICEABLE = "Sliceable";
        public const string SORT_VFX = "VFX";
        public const string SORT_UI = "UI";

        // Physics
        public const float DEFAULT_FRICTION = 0.8f;
        public const float DEFAULT_BOUNCINESS = 0.1f;
        public const float GRAVITY_SCALE = 1.5f;

        // Scoring
        public const int PERFECT_BASE_MULTIPLIER = 3;
        public const int GREAT_BASE_MULTIPLIER = 2;
        public const int GOOD_BASE_MULTIPLIER = 1;
        public const int ON_FIRE_STREAK_THRESHOLD = 5;
        public const int RAINBOW_STREAK_THRESHOLD = 10;

        // Slice Zone (percentage of screen height)
        public const float SLICE_ZONE_DEFAULT_SIZE = 0.25f;
        public const float SLICE_ZONE_Y_POSITION = 0.65f;

        // Stability Thresholds
        public const float STABILITY_NORMAL = 0.6f;
        public const float STABILITY_WARNING = 0.4f;
        public const float STABILITY_ALERT = 0.2f;
        public const float STABILITY_COLLAPSED = 0.0f;

        // Tower
        public const float TOWER_BASE_Y = -4.0f;
        public const float OFF_SCREEN_MARGIN = 2.0f;

        // Ads
        public const int INTERSTITIAL_GAME_INTERVAL = 3;
        public const float MIN_SESSION_FOR_INTERSTITIAL = 30f;
        public const float MIN_TIME_BETWEEN_INTERSTITIALS = 180f;

        // Save
        public const string SAVE_FILE_NAME = "sliceandstack_save.json";
        public const int LEADERBOARD_MAX_ENTRIES = 10;

        // Animation
        public const float SLOW_MO_PERFECT_TIMESCALE = 0.3f;
        public const float SLOW_MO_PERFECT_DURATION = 0.4f;
        public const float SLOW_MO_COLLAPSE_TIMESCALE = 0.2f;
        public const float SLOW_MO_COLLAPSE_DURATION = 1.5f;

        // Scene Names
        public const string SCENE_BOOT = "Boot";
        public const string SCENE_MAIN_MENU = "MainMenu";
        public const string SCENE_GAME = "Game";
    }
}
