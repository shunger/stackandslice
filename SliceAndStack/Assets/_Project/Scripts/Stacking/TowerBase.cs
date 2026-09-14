using UnityEngine;

namespace SliceAndStack.Stacking
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class TowerBase : MonoBehaviour
    {
        [SerializeField] private float _width = 6f;
        [SerializeField] private float _height = 0.4f;

        private void Awake()
        {
            try
            {
                gameObject.tag = Utils.Constants.TAG_TOWER_BASE;
            }
            catch
            {
                Debug.LogWarning("[TowerBase] Tag 'TowerBase' not defined. Add it in Project Settings > Tags and Layers.");
            }

            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            var col = GetComponent<BoxCollider2D>();
            col.size = new Vector2(_width, _height);

            // Add physics material for proper stacking
            var mat = new PhysicsMaterial2D("TowerBaseMat")
            {
                friction = Utils.Constants.DEFAULT_FRICTION,
                bounciness = Utils.Constants.DEFAULT_BOUNCINESS
            };
            col.sharedMaterial = mat;

            transform.position = new Vector3(0f, Utils.Constants.TOWER_BASE_Y, 0f);
        }
    }
}
