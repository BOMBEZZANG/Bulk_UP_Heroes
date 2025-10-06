using UnityEngine;

namespace BulkUpHeroes.Core
{
    /// <summary>
    /// Isometric camera controller similar to Diablo/Hades.
    /// Follows the player with a fixed angled view that shows character models clearly.
    /// </summary>
    public class IsometricCameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform _target; // Player

        [Header("Isometric Settings")]
        [Tooltip("Camera angle looking down (Diablo: 60, Hades: 45)")]
        [SerializeField] private float _angleX = 50f;

        [Tooltip("Camera rotation around player (0 = north, 45 = diagonal)")]
        [SerializeField] private float _angleY = 45f;

        [Tooltip("Distance from target")]
        [SerializeField] private float _distance = 12f;

        [Tooltip("Height offset above target")]
        [SerializeField] private float _heightOffset = 2f;

        [Header("Follow Settings")]
        [Tooltip("How smoothly camera follows (lower = smoother, 0 = instant)")]
        [SerializeField] private float _followSmoothing = 5f;

        [Tooltip("Offset from target position")]
        [SerializeField] private Vector3 _targetOffset = Vector3.zero;

        [Header("Optional: Dynamic Zoom")]
        [SerializeField] private bool _enableDynamicZoom = false;
        [SerializeField] private float _minDistance = 8f;
        [SerializeField] private float _maxDistance = 15f;
        [SerializeField] private float _zoomSpeed = 2f;

        private Vector3 _currentVelocity;
        private float _currentDistance;

        private void Awake()
        {
            // Force unparent camera if it's parented to anything
            if (transform.parent != null)
            {
                Debug.LogWarning($"[IsometricCamera] Camera was parented to {transform.parent.name}, unparenting for free movement!");
                transform.SetParent(null);
            }

            // Auto-find player if not assigned
            if (_target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    _target = player.transform;
                    Debug.Log("[IsometricCamera] Auto-found player target");
                }
                else
                {
                    Debug.LogError("[IsometricCamera] No target assigned and no Player tag found!");
                }
            }
            else
            {
                Debug.Log($"[IsometricCamera] Target assigned: {_target.name}");
            }

            _currentDistance = _distance;

            Debug.Log($"[IsometricCamera] Initialized - Angle X: {_angleX}, Y: {_angleY}, Distance: {_currentDistance}");
        }

        private void Start()
        {
            Debug.Log($"[IsometricCamera] Start - Camera Position: {transform.position}, Rotation: {transform.rotation.eulerAngles}");

            // Set camera to correct position immediately on start (skip smoothing for initial positioning)
            if (_target != null)
            {
                Vector3 initialPosition = CalculateIsometricPosition();
                transform.position = initialPosition;

                Vector3 lookAtPoint = _target.position + _targetOffset;
                transform.LookAt(lookAtPoint);

                Debug.Log($"[IsometricCamera] Set initial position: {initialPosition}, looking at: {lookAtPoint}");
            }
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                Debug.LogWarning("[IsometricCamera] LateUpdate - No target!");
                return;
            }

            // Handle optional zoom controls
            if (_enableDynamicZoom)
            {
                HandleZoom();
            }

            // Calculate desired camera position
            Vector3 desiredPosition = CalculateIsometricPosition();

            // Smooth follow
            if (_followSmoothing > 0)
            {
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    desiredPosition,
                    ref _currentVelocity,
                    1f / _followSmoothing
                );
            }
            else
            {
                transform.position = desiredPosition;
            }

            // Always look at target
            Vector3 lookAtPoint = _target.position + _targetOffset;
            transform.LookAt(lookAtPoint);

            // Debug first few frames
            if (Time.frameCount < 5)
            {
                Debug.Log($"[IsometricCamera] Frame {Time.frameCount} - Desired Pos: {desiredPosition}, Actual Pos: {transform.position}, Looking at: {lookAtPoint}");
            }
        }

        /// <summary>
        /// Calculate camera position for isometric view.
        /// </summary>
        private Vector3 CalculateIsometricPosition()
        {
            // Convert angles to radians
            float angleXRad = _angleX * Mathf.Deg2Rad;
            float angleYRad = _angleY * Mathf.Deg2Rad;

            // Calculate offset from target
            float horizontalDistance = _currentDistance * Mathf.Cos(angleXRad);

            float offsetX = horizontalDistance * Mathf.Sin(angleYRad);
            float offsetZ = horizontalDistance * Mathf.Cos(angleYRad);
            float offsetY = _currentDistance * Mathf.Sin(angleXRad) + _heightOffset;

            // Apply to target position
            Vector3 targetPos = _target.position + _targetOffset;

            return new Vector3(
                targetPos.x - offsetX,
                targetPos.y + offsetY,
                targetPos.z - offsetZ
            );
        }

        /// <summary>
        /// Handle zoom in/out with mouse scroll (optional).
        /// </summary>
        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                _currentDistance -= scroll * _zoomSpeed;
                _currentDistance = Mathf.Clamp(_currentDistance, _minDistance, _maxDistance);
            }
        }

        #region Public Methods
        /// <summary>
        /// Change camera angle on the fly.
        /// </summary>
        public void SetAngle(float angleX, float angleY)
        {
            _angleX = Mathf.Clamp(angleX, 20f, 85f);
            _angleY = angleY;
        }

        /// <summary>
        /// Change camera distance.
        /// </summary>
        public void SetDistance(float distance)
        {
            _distance = distance;
            _currentDistance = distance;
        }

        /// <summary>
        /// Set follow target.
        /// </summary>
        public void SetTarget(Transform target)
        {
            _target = target;
        }
        #endregion

        #region Debug Visualization
        private void OnDrawGizmosSelected()
        {
            if (_target == null) return;

            // Draw line from camera to target
            Gizmos.color = Color.yellow;
            Vector3 targetPos = _target.position + _targetOffset;
            Gizmos.DrawLine(transform.position, targetPos);
            Gizmos.DrawWireSphere(targetPos, 0.5f);

            // Draw camera frustum direction
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.forward * 3f);
        }
        #endregion
    }
}
