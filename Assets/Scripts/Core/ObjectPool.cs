using System.Collections.Generic;
using UnityEngine;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.Core
{
    /// <summary>
    /// Generic object pooling system for performance optimization.
    /// Reduces garbage collection by reusing GameObjects instead of destroying them.
    ///
    /// Essential for mobile performance - prevents frame drops from instantiate/destroy calls.
    ///
    /// Phase 1: Basic pooling for enemies
    /// Future: Will pool pickups, particles, and UI elements
    /// </summary>
    /// <typeparam name="T">Type that implements IPoolable</typeparam>
    public class ObjectPool<T> where T : IPoolable
    {
        #region Pool Properties
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly int _initialSize;
        private readonly bool _allowGrowth;

        private Queue<T> _availableObjects;
        private List<T> _allObjects;
        private int _totalCreated = 0;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new object pool.
        /// </summary>
        /// <param name="prefab">Prefab to pool</param>
        /// <param name="initialSize">Initial pool size</param>
        /// <param name="parent">Parent transform for pooled objects</param>
        /// <param name="allowGrowth">Allow pool to grow beyond initial size</param>
        public ObjectPool(GameObject prefab, int initialSize, Transform parent = null, bool allowGrowth = true)
        {
            _prefab = prefab;
            _initialSize = initialSize;
            _parent = parent;
            _allowGrowth = allowGrowth;

            _availableObjects = new Queue<T>(initialSize);
            _allObjects = new List<T>(initialSize);

            // Pre-populate pool
            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }

            // Force physics sync to complete initialization
            Physics.SyncTransforms();

            Debug.Log($"[ObjectPool] Created pool for {prefab.name} with {initialSize} objects");
        }
        #endregion

        #region Pool Management
        /// <summary>
        /// Get an object from the pool.
        /// </summary>
        /// <param name="position">World position to spawn at</param>
        /// <param name="rotation">World rotation to spawn with</param>
        /// <returns>Pooled object instance</returns>
        public T Get(Vector3 position, Quaternion rotation)
        {
            T obj;

            // Try to get from available objects
            if (_availableObjects.Count > 0)
            {
                obj = _availableObjects.Dequeue();
            }
            else if (_allowGrowth)
            {
                // Create new object if pool can grow
                Debug.LogWarning($"[ObjectPool] Pool for {_prefab.name} is empty, creating new instance");
                obj = CreateNewObject();
            }
            else
            {
                // Pool exhausted and can't grow
                Debug.LogError($"[ObjectPool] Pool for {_prefab.name} is exhausted!");
                return default(T);
            }

            // Setup object
            GameObject gameObject = obj.GameObject;
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
            gameObject.SetActive(true);

            // Call poolable callback
            obj.OnSpawnFromPool();

            return obj;
        }

        /// <summary>
        /// Return an object to the pool.
        /// </summary>
        /// <param name="obj">Object to return</param>
        public void Return(T obj)
        {
            if (obj == null) return;

            try
            {
                // Call poolable callback
                obj.OnReturnToPool();

                // Deactivate and parent
                GameObject gameObject = obj.GameObject;
                if (gameObject == null)
                {
                    Debug.LogWarning($"[ObjectPool] Cannot return object - GameObject is null");
                    return;
                }

                gameObject.SetActive(false);

                if (_parent != null)
                {
                    gameObject.transform.SetParent(_parent);
                }

                // Return to pool
                _availableObjects.Enqueue(obj);
            }
            catch (MissingReferenceException)
            {
                Debug.LogWarning($"[ObjectPool] Cannot return destroyed object to pool");
            }
        }

        /// <summary>
        /// Return an object to the pool by GameObject reference.
        /// </summary>
        public void Return(GameObject gameObject)
        {
            T obj = gameObject.GetComponent<T>();
            if (obj != null)
            {
                Return(obj);
            }
            else
            {
                Debug.LogWarning($"[ObjectPool] GameObject {gameObject.name} does not have required component");
            }
        }

        /// <summary>
        /// Create a new object for the pool.
        /// </summary>
        private T CreateNewObject()
        {
            GameObject instance = Object.Instantiate(_prefab, _parent);
            instance.name = $"{_prefab.name}_{_totalCreated}";

            // Keep active initially to ensure full initialization (materials, shaders, etc.)
            instance.SetActive(true);

            T component = instance.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"[ObjectPool] Prefab {_prefab.name} does not have required IPoolable component!");
                Object.Destroy(instance);
                return default(T);
            }

            _allObjects.Add(component);
            _availableObjects.Enqueue(component);
            _totalCreated++;

            // Now deactivate after initialization is complete
            instance.SetActive(false);

            return component;
        }
        #endregion

        #region Pool Information
        /// <summary>
        /// Get count of available objects in pool.
        /// </summary>
        public int AvailableCount => _availableObjects.Count;

        /// <summary>
        /// Get count of all objects created by this pool.
        /// </summary>
        public int TotalCount => _allObjects.Count;

        /// <summary>
        /// Get count of objects currently in use.
        /// </summary>
        public int ActiveCount => _allObjects.Count - _availableObjects.Count;
        #endregion

        #region Cleanup
        /// <summary>
        /// Clear the pool and destroy all objects.
        /// </summary>
        public void Clear()
        {
            // Create a copy to avoid modification during iteration
            var objectsToDestroy = new List<T>(_allObjects);

            foreach (T obj in objectsToDestroy)
            {
                // Safe null check for Unity objects (handles destroyed objects)
                if (obj != null)
                {
                    try
                    {
                        GameObject go = obj.GameObject;
                        if (go != null)
                        {
                            Object.Destroy(go);
                        }
                    }
                    catch (MissingReferenceException)
                    {
                        // Object was already destroyed, ignore
                    }
                }
            }

            _availableObjects.Clear();
            _allObjects.Clear();
            _totalCreated = 0;

            Debug.Log($"[ObjectPool] Cleared pool for {_prefab.name}");
        }
        #endregion
    }
}
