using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Spawning
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class WindAffector : MonoBehaviour
    {
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (!ServiceLocator.TryGet<Difficulty.DifficultyManager>(out var difficulty)) return;

            var windForce = difficulty.GetWindForce();
            if (windForce.sqrMagnitude > 0.01f)
            {
                _rb.AddForce(windForce * _rb.mass, ForceMode2D.Force);
            }
        }
    }
}
