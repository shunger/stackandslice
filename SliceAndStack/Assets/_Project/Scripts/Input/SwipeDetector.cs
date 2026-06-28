using System;
using UnityEngine;

namespace SliceAndStack.Input
{
    public class SwipeDetector
    {
        public event Action<Vector2, Vector2, float> OnSwipe; // start, end, speed

        private float _minSwipeDistance = 50f;
        private float _maxSwipeDuration = 0.5f;
        private float _touchStartTime;
        private Vector2 _touchStartPos;
        private bool _tracking;

        public void ProcessTouchDown(Vector2 screenPosition)
        {
            _touchStartTime = Time.realtimeSinceStartup;
            _touchStartPos = screenPosition;
            _tracking = true;
        }

        public void ProcessTouchUp(Vector2 screenPosition)
        {
            if (!_tracking) return;
            _tracking = false;

            float duration = Time.realtimeSinceStartup - _touchStartTime;
            float distance = Vector2.Distance(_touchStartPos, screenPosition);

            if (distance >= _minSwipeDistance && duration <= _maxSwipeDuration)
            {
                float speed = distance / duration;
                OnSwipe?.Invoke(_touchStartPos, screenPosition, speed);
            }
        }

        public void ProcessTouchCancel()
        {
            _tracking = false;
        }
    }
}
