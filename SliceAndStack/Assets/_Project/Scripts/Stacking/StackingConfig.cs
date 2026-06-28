using UnityEngine;

namespace SliceAndStack.Stacking
{
    [CreateAssetMenu(fileName = "StackingConfig", menuName = "SliceAndStack/Config/Stacking Config")]
    public class StackingConfig : ScriptableObject
    {
        [Header("Physics Material")]
        public float friction = 0.8f;
        public float bounciness = 0.1f;

        [Header("Stability Detection")]
        [Tooltip("Max horizontal offset of center of mass from base center before instability")]
        public float maxCenterOfMassOffset = 1.5f;

        [Tooltip("Max angular velocity of any piece before instability")]
        public float maxAngularVelocity = 200f;

        [Tooltip("Weight of center-of-mass offset in stability calculation (0-1)")]
        [Range(0f, 1f)] public float comWeightInStability = 0.6f;

        [Tooltip("Weight of angular velocity in stability calculation (0-1)")]
        [Range(0f, 1f)] public float angularWeightInStability = 0.4f;

        [Header("Stability Thresholds")]
        public float warningThreshold = 0.6f;
        public float alertThreshold = 0.4f;
        public float criticalThreshold = 0.2f;

        [Header("Collapse Detection")]
        [Tooltip("How long stability must stay at 0 before triggering game over")]
        public float collapseConfirmTime = 0.5f;

        [Tooltip("Y position below which a piece is considered fallen off")]
        public float fallOffY = -6f;

        [Header("Tower State Snapshot")]
        [Tooltip("How often to save tower state for revive (seconds)")]
        public float snapshotInterval = 3f;

        [Header("Landing")]
        public float landingVelocityThreshold = 0.5f;
        public float squashAmount = 0.15f;
        public float squashDuration = 0.2f;
    }
}
