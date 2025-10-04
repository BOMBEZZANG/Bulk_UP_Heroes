using System.Collections;
using UnityEngine;
using BulkUpHeroes.Utils;
using BulkUpHeroes.Core;

namespace BulkUpHeroes.Parts
{
    /// <summary>
    /// Handles part pickup collision, visual effects, and despawn timer.
    /// Attached to dropped part objects in the world.
    ///
    /// Phase 3: Auto-pickup on collision with player
    /// Future: May add manual pickup button option
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class PartPickup : MonoBehaviour, IPoolable
    {
        #region Configuration
        [Header("Part Data")]
        [SerializeField] private PartData _partData;

        [Header("Visual Settings")]
        [SerializeField] private float _hoverHeight = 0.5f;
        [SerializeField] private float _hoverSpeed = 1f;
        [SerializeField] private float _rotationSpeed = 45f;
        [SerializeField] private float _bounceAmplitude = 0.1f;

        [Header("Despawn Settings")]
        [SerializeField] private float _despawnTime = 5f;
        [SerializeField] private float _fadeDuration = 0.5f;
        #endregion

        #region Components
        private Collider _collider;
        private MeshRenderer _meshRenderer;
        private Material _material;
        private Transform _transform;
        #endregion

        #region State
        private float _spawnTime;
        private Vector3 _spawnPosition;
        private bool _isPickedUp = false;
        private Color _originalColor;
        #endregion

        #region Properties
        public PartData PartData => _partData;
        public GameObject GameObject => gameObject;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void Update()
        {
            if (!_isPickedUp)
            {
                UpdateVisualEffects();
                CheckDespawnTimer();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleCollision(other);
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize components and cache references.
        /// </summary>
        private void InitializeComponents()
        {
            _transform = transform;
            _collider = GetComponent<Collider>();
            _meshRenderer = GetComponent<MeshRenderer>();

            if (_meshRenderer != null)
            {
                _material = _meshRenderer.material;
                _originalColor = _material.color;
            }

            // Setup as trigger
            _collider.isTrigger = true;

            // Set layer to Pickup
            gameObject.layer = LayerMask.NameToLayer("Default"); // Will be "Pickup" layer in setup
        }

        /// <summary>
        /// Initialize with specific part data.
        /// </summary>
        public void Initialize(PartData partData, Vector3 position)
        {
            _partData = partData;
            _spawnPosition = position;
            _transform.position = position + Vector3.up * _hoverHeight;
            _spawnTime = Time.time;
            _isPickedUp = false;

            // Apply visual properties
            if (_meshRenderer != null && _partData != null)
            {
                _material.color = _partData.partColor;
                _originalColor = _partData.partColor;
                _transform.localScale = Vector3.one * _partData.GetRarityScale();
            }

            // Enable collider
            _collider.enabled = true;
        }
        #endregion

        #region Visual Effects
        /// <summary>
        /// Update hover, rotation, and bounce animations.
        /// </summary>
        private void UpdateVisualEffects()
        {
            // Rotate continuously
            _transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.World);

            // Gentle bounce
            float bounce = Mathf.Sin(Time.time * _hoverSpeed) * _bounceAmplitude;
            _transform.position = _spawnPosition + Vector3.up * (_hoverHeight + bounce);

            // Pulse glow for rare/epic
            if (_partData != null && _partData.rarity != Rarity.Common)
            {
                float pulse = 0.8f + Mathf.Sin(Time.time * 2f) * 0.2f;
                _material.color = _originalColor * pulse;
            }
        }
        #endregion

        #region Despawn Timer
        /// <summary>
        /// Check if pickup should despawn.
        /// </summary>
        private void CheckDespawnTimer()
        {
            float timeAlive = Time.time - _spawnTime;

            if (timeAlive >= _despawnTime)
            {
                StartCoroutine(FadeOutAndDespawn());
            }
            else if (timeAlive >= _despawnTime - _fadeDuration)
            {
                // Start fading
                float fadeProgress = (timeAlive - (_despawnTime - _fadeDuration)) / _fadeDuration;
                Color fadeColor = _originalColor;
                fadeColor.a = 1f - fadeProgress;
                _material.color = fadeColor;
            }
        }

        /// <summary>
        /// Fade out and return to pool.
        /// </summary>
        private IEnumerator FadeOutAndDespawn()
        {
            float elapsed = 0f;

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = 1f - (elapsed / _fadeDuration);

                Color fadeColor = _originalColor;
                fadeColor.a = alpha;
                _material.color = fadeColor;

                yield return null;
            }

            ReturnToPool();
        }
        #endregion

        #region Collision Handling
        /// <summary>
        /// Handle collision with player.
        /// </summary>
        private void HandleCollision(Collider other)
        {
            if (_isPickedUp) return;

            // Check if player
            if (!other.CompareTag(GameConstants.TAG_PLAYER)) return;

            // Try to get CharacterPartManager
            var partManager = other.GetComponent<Player.CharacterPartManager>();
            if (partManager == null)
            {
                Debug.LogWarning("[PartPickup] Player has no CharacterPartManager!");
                return;
            }

            // Attempt pickup
            bool success = partManager.TryEquipPart(_partData);

            if (success)
            {
                _isPickedUp = true;
                Debug.Log($"[PartPickup] Player picked up {_partData.displayName}");

                // Trigger pickup event
                GameEvents.TriggerPartPickedUp(_partData);

                // Visual/audio feedback
                SpawnPickupEffect();

                // Return to pool
                ReturnToPool();
            }
            else
            {
                Debug.Log($"[PartPickup] Player already has better {_partData.partType}");
                // Show "Already have better" message
                ShowAlreadyHaveMessage();
            }
        }

        /// <summary>
        /// Spawn visual effect on pickup.
        /// </summary>
        private void SpawnPickupEffect()
        {
            // Future: Particle effect burst
            // Future: Sound effect based on rarity
            Debug.Log($"[PartPickup] Pickup effect for {_partData.rarity} part");
        }

        /// <summary>
        /// Show message when player has better part.
        /// </summary>
        private void ShowAlreadyHaveMessage()
        {
            // Future: Show UI message
            Debug.Log($"[PartPickup] Already have better part");
        }
        #endregion

        #region IPoolable Implementation
        /// <summary>
        /// Called when spawned from pool.
        /// </summary>
        public void OnSpawnFromPool()
        {
            gameObject.SetActive(true);
            _isPickedUp = false;
            _collider.enabled = true;

            // Reset material
            if (_material != null)
            {
                Color resetColor = _originalColor;
                resetColor.a = 1f;
                _material.color = resetColor;
            }
        }

        /// <summary>
        /// Called when returned to pool.
        /// </summary>
        public void OnReturnToPool()
        {
            StopAllCoroutines();
            _isPickedUp = false;
            _collider.enabled = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Return this pickup to pool.
        /// </summary>
        private void ReturnToPool()
        {
            if (PartDropManager.Instance != null)
            {
                PartDropManager.Instance.ReturnPickupToPool(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}
