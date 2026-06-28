using UnityEngine;

namespace SliceAndStack.Stacking
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class TowerBase : MonoBehaviour
    {
        [SerializeField] private float _width = 3f;
        [SerializeField] private float _height = 0.3f;

        private void Awake()
        {
            gameObject.tag = Utils.Constants.TAG_TOWER_BASE;

            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            var col = GetComponent<BoxCollider2D>();
            col.size = new Vector2(_width, _height);

            transform.position = new Vector3(0f, Utils.Constants.TOWER_BASE_Y, 0f);
        }
    }
}
