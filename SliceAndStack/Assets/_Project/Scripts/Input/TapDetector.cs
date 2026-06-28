using System;
using UnityEngine;

namespace SliceAndStack.Input
{
    public class TapDetector
    {
        public event Action<Vector2> OnTap;

        private float _maxTapDuration = 0.3f;
        private float _maxTapMovement = 20f;
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
            float movement = Vector2.Distance(_touchStartPos, screenPosition);

            if (duration <= _maxTapDuration && movement <= _maxTapMovement)
            {
                OnTap?.Invoke(screenPosition);
            }
        }

        public void ProcessTouchCancel()
        {
            _tracking = false;
        }
    }
}
