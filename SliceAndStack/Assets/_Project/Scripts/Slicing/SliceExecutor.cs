using UnityEngine;
using System.Collections;
using SliceAndStack.Core;
using SliceAndStack.Spawning;

namespace SliceAndStack.Slicing
{
    public class SliceExecutor : MonoBehaviour
    {
        [SerializeField] private SliceConfig _config;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<SliceExecutor>();
        }

        public void Execute(SliceResult result)
        {
            if (!result.IsValid) return;

            var target = result.Target;
            var data = target.Data;
            var position = target.transform.position;

            target.MarkSliced();

            var pool = ServiceLocator.Get<ObjectPool>();
            if (pool == null) return;

            // Create left half
            var leftHalf = pool.GetSlicedHalf();
            SetupHalf(leftHalf, data.leftHalfSprite, position + (Vector2)data.leftHalfOffset,
                -_config.sliceSeparationForce, _config.sliceUpwardForce, -_config.sliceTorque, data.mass);

            // Create right half
            var rightHalf = pool.GetSlicedHalf();
            SetupHalf(rightHalf, data.rightHalfSprite, position + (Vector2)data.rightHalfOffset,
                _config.sliceSeparationForce, _config.sliceUpwardForce, _config.sliceTorque, data.mass);

            // Publish slice event
            EventBus.Publish(new SlicePerformedEvent
            {
                Quality = result.Quality,
                SlicePosition = position,
                LeftHalf = leftHalf,
                RightHalf = rightHalf
            });

            // Return the original object
            target.ReturnToPool();

            // Start gravity on halves after delay
            StartCoroutine(EnableHalfGravity(leftHalf, rightHalf));
        }

        private void SetupHalf(GameObject half, Sprite sprite, Vector2 position,
            float horizontalForce, float upwardForce, float torque, float mass)
        {
            half.transform.position = position;
            half.transform.rotation = Quaternion.identity;

            var sr = half.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = sprite;
                sr.color = Color.white;
            }

            var rb = half.GetComponent<Rigidbody2D>();
            if (rb == null) rb = half.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = mass * 0.5f;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // Apply slice forces
            rb.AddForce(new Vector2(horizontalForce, upwardForce), ForceMode2D.Impulse);
            rb.AddTorque(torque);

            var col = half.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;

            // Add stacked piece component for tower interaction
            var stackedPiece = half.GetComponent<Stacking.StackedPiece>();
            if (stackedPiece == null)
                stackedPiece = half.AddComponent<Stacking.StackedPiece>();
            stackedPiece.Initialize(mass * 0.5f);
        }

        private IEnumerator EnableHalfGravity(GameObject left, GameObject right)
        {
            yield return new WaitForSeconds(_config.halfGravityDelay);

            EnableGravity(left);
            EnableGravity(right);
        }

        private void EnableGravity(GameObject obj)
        {
            if (obj == null || !obj.activeInHierarchy) return;
            var rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = Utils.Constants.GRAVITY_SCALE;
            }
        }
    }
}
