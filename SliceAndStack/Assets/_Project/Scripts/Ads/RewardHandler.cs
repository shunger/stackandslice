using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Ads
{
    public class RewardHandler : MonoBehaviour
    {
        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public void HandleReward(RewardType type)
        {
            switch (type)
            {
                case RewardType.Revive:
                    HandleRevive();
                    break;
                case RewardType.DoubleScore:
                    HandleDoubleScore();
                    break;
            }

            EventBus.Publish(new AdRewardedEvent { Type = type });
        }

        private void HandleRevive()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.Revive();
        }

        private void HandleDoubleScore()
        {
            if (ServiceLocator.TryGet<Scoring.ScoreManager>(out var scoreManager))
                scoreManager.DoubleScore();
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<RewardHandler>();
        }
    }
}
