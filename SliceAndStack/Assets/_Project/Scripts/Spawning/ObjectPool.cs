using System.Collections.Generic;
using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Spawning
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private int _initialPoolSize = 10;
        [SerializeField] private GameObject _sliceablePrefab;
        [SerializeField] private GameObject _slicedHalfPrefab;

        private readonly Queue<GameObject> _sliceablePool = new();
        private readonly Queue<GameObject> _slicedHalfPool = new();
        private Transform _poolParent;

        private void Awake()
        {
            _poolParent = new GameObject("ObjectPool").transform;
            _poolParent.SetParent(transform);
            ServiceLocator.Register(this);
            Prewarm();
        }

        private void Prewarm()
        {
            for (int i = 0; i < _initialPoolSize; i++)
            {
                var obj = CreateSliceable();
                obj.SetActive(false);
                _sliceablePool.Enqueue(obj);
            }

            for (int i = 0; i < _initialPoolSize * 2; i++)
            {
                var obj = CreateSlicedHalf();
                obj.SetActive(false);
                _slicedHalfPool.Enqueue(obj);
            }
        }

        private GameObject CreateSliceable()
        {
            var obj = Instantiate(_sliceablePrefab, _poolParent);
            if (obj.GetComponent<SliceableObject>() == null)
                obj.AddComponent<SliceableObject>();
            return obj;
        }

        private GameObject CreateSlicedHalf()
        {
            var obj = Instantiate(_slicedHalfPrefab, _poolParent);
            return obj;
        }

        public GameObject GetSliceable()
        {
            GameObject obj;
            if (_sliceablePool.Count > 0)
            {
                obj = _sliceablePool.Dequeue();
            }
            else
            {
                obj = CreateSliceable();
            }
            obj.SetActive(true);
            var sliceable = obj.GetComponent<SliceableObject>();
            if (sliceable != null) sliceable.ResetState();
            return obj;
        }

        public GameObject GetSlicedHalf()
        {
            GameObject obj;
            if (_slicedHalfPool.Count > 0)
            {
                obj = _slicedHalfPool.Dequeue();
            }
            else
            {
                obj = CreateSlicedHalf();
            }
            obj.SetActive(true);
            return obj;
        }

        public void Return(GameObject obj)
        {
            if (obj == null) return;
            obj.SetActive(false);
            obj.transform.SetParent(_poolParent);

            if (obj.GetComponent<SliceableObject>() != null)
                _sliceablePool.Enqueue(obj);
            else
                _slicedHalfPool.Enqueue(obj);
        }

        public void ReturnAll()
        {
            // Collect active sliceables and halves from the scene
            foreach (var sliceable in FindObjectsByType<SliceableObject>(FindObjectsSortMode.None))
            {
                if (sliceable.gameObject.activeInHierarchy)
                    Return(sliceable.gameObject);
            }
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<ObjectPool>();
        }
    }
}
