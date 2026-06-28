using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Stacking
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class StackedPiece : MonoBehaviour
    {
        public bool IsStacked { get; private set; }
        public float Mass { get; private set; }
        public Rigidbody2D Rb { get; private set; }

        private bool _hasLanded;
        private float _settleTimer;
        private const float SETTLE_TIME = 0.3f;
        private const float SETTLE_VELOCITY = 0.1f;

        public void Initialize(float mass)
        {
            Mass = mass;
            IsStacked = false;
            _hasLanded = false;
            _settleTimer = 0f;

            Rb = GetComponent<Rigidbody2D>();
            Rb.mass = mass;

            // Apply physics material settings
            Rb.sharedMaterial = CreatePhysicsMaterial();
        }

        private PhysicsMaterial2D CreatePhysicsMaterial()
        {
            var mat = new PhysicsMaterial2D("StackedPieceMat")
            {
                friction = Utils.Constants.DEFAULT_FRICTION,
                bounciness = Utils.Constants.DEFAULT_BOUNCINESS
            };
            return mat;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_hasLanded) return;

            // Check if landed on tower base or another stacked piece
            bool landedOnTower = collision.gameObject.CompareTag(Utils.Constants.TAG_TOWER_BASE) ||
                                 collision.gameObject.GetComponent<StackedPiece>() != null;

            if (landedOnTower)
            {
                _hasLanded = true;
                float impactForce = collision.relativeVelocity.magnitude;

                if (ServiceLocator.TryGet<TowerManager>(out var tower))
                {
                    tower.RegisterPiece(this);
                }

                EventBus.Publish(new PieceLandedEvent
                {
                    Piece = gameObject,
                    ImpactForce = impactForce,
                    TowerHeight = transform.position.y - Utils.Constants.TOWER_BASE_Y
                });
            }
        }

        private void FixedUpdate()
        {
            if (!_hasLanded || IsStacked) return;

            // Check if piece has settled
            if (Rb.linearVelocity.magnitude < SETTLE_VELOCITY &&
                Mathf.Abs(Rb.angularVelocity) < 5f)
            {
                _settleTimer += Time.fixedDeltaTime;
                if (_settleTimer >= SETTLE_TIME)
                {
                    IsStacked = true;
                }
            }
            else
            {
                _settleTimer = 0f;
            }
        }

        public PieceSnapshot TakeSnapshot()
        {
            return new PieceSnapshot
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Velocity = Rb.linearVelocity,
                AngularVelocity = Rb.angularVelocity
            };
        }

        public void RestoreSnapshot(PieceSnapshot snapshot)
        {
            transform.position = snapshot.Position;
            transform.rotation = snapshot.Rotation;
            Rb.linearVelocity = snapshot.Velocity;
            Rb.angularVelocity = snapshot.AngularVelocity;
            IsStacked = true;
            _hasLanded = true;
        }
    }

    public struct PieceSnapshot
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector2 Velocity;
        public float AngularVelocity;
    }
}
