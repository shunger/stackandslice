using UnityEngine;
using SliceAndStack.Utils;
using SliceAndStack.Spawning;

namespace SliceAndStack.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        private Sprite _squareSprite;
        private Sprite _circleSprite;
        private Sprite _circleLeft;
        private Sprite _circleRight;

        private void Awake()
        {
            SetupCamera();
            CreatePlaceholderSprites();
            SetupTowerBase();
            SetupSliceZoneVisual();
        }

        private void Start()
        {
            AssignSpritesToData();

            // Delay start to next frame so all Start() calls finish first
            StartCoroutine(AutoStartGame());
        }

        private System.Collections.IEnumerator AutoStartGame()
        {
            yield return null; // wait one frame
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
                Debug.Log("[GameBootstrapper] Auto-started game");
            }
        }

        private void SetupCamera()
        {
            var cam = Camera.main;
            if (cam == null) return;

            cam.orthographic = true;
            cam.orthographicSize = 10f;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.2f);
            cam.transform.position = new Vector3(0f, 3f, -10f);
        }

        private void CreatePlaceholderSprites()
        {
            // White square
            var tex = new Texture2D(64, 64);
            var colors = new Color[64 * 64];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.white;
            tex.SetPixels(colors);
            tex.Apply();
            _squareSprite = Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);

            // Circle
            var circleTex = new Texture2D(64, 64);
            var circleColors = new Color[64 * 64];
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    float dx = x - 32f;
                    float dy = y - 32f;
                    circleColors[y * 64 + x] = (dx * dx + dy * dy <= 32 * 32) ? Color.white : Color.clear;
                }
            }
            circleTex.SetPixels(circleColors);
            circleTex.Apply();

            _circleSprite = Sprite.Create(circleTex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
            _circleLeft = Sprite.Create(circleTex, new Rect(0, 0, 32, 64), new Vector2(1f, 0.5f), 64);
            _circleRight = Sprite.Create(circleTex, new Rect(32, 0, 32, 64), new Vector2(0f, 0.5f), 64);
        }

        private void AssignSpritesToData()
        {
            // Find SpawnManager and assign sprites to its data list directly
            var spawnManager = ServiceLocator.Get<SpawnManager>();
            if (spawnManager == null)
            {
                Debug.LogWarning("[GameBootstrapper] SpawnManager not found");
                return;
            }

            // Use reflection to access the private list, or find all SliceableData in the project
            var allData = Resources.FindObjectsOfTypeAll<SliceableData>();
            Debug.Log($"[GameBootstrapper] Found {allData.Length} SliceableData assets");

            foreach (var data in allData)
            {
                if (data.wholeSprite == null)
                {
                    data.wholeSprite = _circleSprite;
                    data.leftHalfSprite = _circleLeft;
                    data.rightHalfSprite = _circleRight;
                    data.sliceTrailColor = new Color(1f, 0.3f, 0.3f);
                    Debug.Log($"[GameBootstrapper] Assigned sprites to: {data.objectName}");
                }
            }

            // Also set sprites on the prefabs in ObjectPool
            if (ServiceLocator.TryGet<ObjectPool>(out var pool))
            {
                // Spawn a test object to verify it gets sprites
                Debug.Log("[GameBootstrapper] ObjectPool found");
            }
        }

        private void SetupTowerBase()
        {
            var towerBase = FindFirstObjectByType<Stacking.TowerBase>();
            if (towerBase != null)
            {
                var sr = towerBase.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite == null)
                {
                    sr.sprite = _squareSprite;
                    sr.color = new Color(0.4f, 0.4f, 0.5f);
                    towerBase.transform.localScale = new Vector3(3f, 0.3f, 1f);
                }
            }
        }

        private void SetupSliceZoneVisual()
        {
            var sliceZone = FindFirstObjectByType<Slicing.SliceZoneVisual>();
            if (sliceZone != null)
            {
                var sr = sliceZone.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite == null)
                {
                    sr.sprite = _squareSprite;
                    sr.color = new Color(1f, 1f, 1f, 0.1f);
                }
            }
        }
    }
}
