using UnityEngine;

namespace SliceAndStack.Ads
{
    [CreateAssetMenu(fileName = "AdConfig", menuName = "SliceAndStack/Config/Ad Config")]
    public class AdConfig : ScriptableObject
    {
        [Header("AdMob IDs - Test")]
        public string testInterstitialId = "ca-app-pub-3940256099942544/1033173712";
        public string testRewardedId = "ca-app-pub-3940256099942544/5224354917";

        [Header("AdMob IDs - Production")]
        public string productionInterstitialIdAndroid = "";
        public string productionRewardedIdAndroid = "";
        public string productionInterstitialIdiOS = "";
        public string productionRewardedIdiOS = "";

        [Header("Frequency Capping")]
        public int interstitialGameInterval = 3;
        public float minSessionDurationForInterstitial = 30f;
        public float minTimeBetweenInterstitials = 180f;

        [Header("Revive")]
        public int maxRevivesPerGame = 1;

        [Header("Mode")]
        public bool useTestAds = true;

        public string InterstitialId
        {
            get
            {
                if (useTestAds) return testInterstitialId;
#if UNITY_ANDROID
                return productionInterstitialIdAndroid;
#elif UNITY_IOS
                return productionInterstitialIdiOS;
#else
                return testInterstitialId;
#endif
            }
        }

        public string RewardedId
        {
            get
            {
                if (useTestAds) return testRewardedId;
#if UNITY_ANDROID
                return productionRewardedIdAndroid;
#elif UNITY_IOS
                return productionRewardedIdiOS;
#else
                return testRewardedId;
#endif
            }
        }
    }
}
