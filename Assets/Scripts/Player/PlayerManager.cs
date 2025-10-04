using UnityEngine;
using BulkUpHeroes.Utils;
using BulkUpHeroes.Core;

namespace BulkUpHeroes.Player
{
    /// <summary>
    /// Singleton manager for player reference caching.
    /// Prevents expensive GameObject.FindGameObjectWithTag() calls during gameplay.
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        #region Singleton
        private static PlayerManager _instance;
        public static PlayerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PlayerManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("PlayerManager");
                        _instance = go.AddComponent<PlayerManager>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Properties
        private Transform _playerTransform;
        private GameObject _playerGameObject;

        public static Transform PlayerTransform => Instance._playerTransform;
        public static GameObject PlayerGameObject => Instance._playerGameObject;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Singleton enforcement
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            CachePlayerReference();
        }

        private void OnEnable()
        {
            GameEvents.OnGameStart += HandleGameStart;
            GameEvents.OnGameRestart += HandleGameRestart;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= HandleGameStart;
            GameEvents.OnGameRestart -= HandleGameRestart;
        }
        #endregion

        #region Player Reference Management
        /// <summary>
        /// Cache player reference for fast access.
        /// </summary>
        private void CachePlayerReference()
        {
            GameObject player = GameObject.FindGameObjectWithTag(GameConstants.TAG_PLAYER);

            if (player != null)
            {
                _playerGameObject = player;
                _playerTransform = player.transform;
                Debug.Log("[PlayerManager] Player reference cached");
            }
        }

        /// <summary>
        /// Manually register player reference.
        /// Call this after player is spawned.
        /// </summary>
        public static void RegisterPlayer(GameObject player)
        {
            if (player == null) return;

            Instance._playerGameObject = player;
            Instance._playerTransform = player.transform;
            Debug.Log("[PlayerManager] Player manually registered");
        }

        /// <summary>
        /// Clear player reference.
        /// </summary>
        public static void ClearPlayer()
        {
            Instance._playerGameObject = null;
            Instance._playerTransform = null;
        }
        #endregion

        #region Event Handlers
        private void HandleGameStart()
        {
            CachePlayerReference();
        }

        private void HandleGameRestart()
        {
            ClearPlayer();
            // Player will be registered after spawn
        }
        #endregion

        #region Public Helper Methods
        /// <summary>
        /// Check if player reference is valid.
        /// </summary>
        public static bool HasPlayer()
        {
            return Instance._playerGameObject != null && Instance._playerTransform != null;
        }

        /// <summary>
        /// Get distance from a position to the player.
        /// </summary>
        public static float GetDistanceToPlayer(Vector3 position)
        {
            if (!HasPlayer()) return float.MaxValue;
            return Vector3.Distance(position, Instance._playerTransform.position);
        }
        #endregion
    }
}
