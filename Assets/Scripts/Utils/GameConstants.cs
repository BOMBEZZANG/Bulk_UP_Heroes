using UnityEngine;

namespace BulkUpHeroes.Utils
{
    /// <summary>
    /// Central repository for game constants and configuration values.
    /// This makes balancing and tweaking values easier.
    /// </summary>
    public static class GameConstants
    {
        #region Arena Settings
        public const float ARENA_SIZE = 10f;
        public const float ARENA_HALF_SIZE = ARENA_SIZE / 2f;
        public const float WALL_HEIGHT = 2f;
        public const float SPAWN_POINT_OFFSET = 1f; // Distance from walls
        #endregion

        #region Player Settings
        // Phase 3: Reduced base stats to make parts feel impactful
        public const float PLAYER_BASE_SPEED = 4f;
        public const float PLAYER_BASE_HEALTH = 500f;
        public const float PLAYER_BASE_DAMAGE = 3f;
        public const float PLAYER_BASE_ATTACK_SPEED = 0.5f;
        public const float PLAYER_SPAWN_HEIGHT = 0.5f;
        public const float PLAYER_ROTATION_SPEED = 720f; // Degrees per second
        #endregion

        #region Enemy Settings
        public const float ENEMY_BASE_SPEED = 2f;
        public const float ENEMY_BASE_HEALTH = 20f;
        public const float ENEMY_BASE_DAMAGE = 5f;
        public const float ENEMY_ATTACK_RANGE = 1f;
        public const float ENEMY_SPAWN_HEIGHT = 0.5f;
        public const int MAX_ENEMIES_PER_WAVE = 10;
        public const float ENEMY_SPAWN_DELAY = 0.5f;
        #endregion

        #region Combat Settings
        public const float ATTACK_RANGE = 1.5f;
        public const float TARGET_UPDATE_INTERVAL = 0.2f; // How often to search for targets
        #endregion

        #region Input Settings
        public const float JOYSTICK_DEAD_ZONE = 0.1f;
        public const float JOYSTICK_SIZE = 150f;
        public const float JOYSTICK_ALPHA = 0.5f;
        #endregion

        #region Performance Settings
        public const int ENEMY_POOL_SIZE = 20;
        public const int PICKUP_POOL_SIZE = 10;
        public const int TARGET_FRAME_RATE = 60;
        #endregion

        #region Layer Names (Must match Unity Layer settings)
        public const string LAYER_PLAYER = "Player";
        public const string LAYER_ENEMY = "Enemy";
        public const string LAYER_WALL = "Wall";
        public const string LAYER_FLOOR = "Floor";
        public const string LAYER_PICKUP = "Pickup";

        // Cached LayerMasks (initialized at runtime for performance)
        private static int _playerLayerMask = -1;
        private static int _enemyLayerMask = -1;

        public static int PlayerLayerMask
        {
            get
            {
                if (_playerLayerMask == -1)
                    _playerLayerMask = LayerMask.GetMask(LAYER_PLAYER);
                return _playerLayerMask;
            }
        }

        public static int EnemyLayerMask
        {
            get
            {
                if (_enemyLayerMask == -1)
                    _enemyLayerMask = LayerMask.GetMask(LAYER_ENEMY);
                return _enemyLayerMask;
            }
        }
        #endregion

        #region Tag Names (Must match Unity Tag settings)
        public const string TAG_PLAYER = "Player";
        public const string TAG_ENEMY = "Enemy";
        public const string TAG_WALL = "Wall";
        public const string TAG_SPAWN_POINT = "SpawnPoint";
        #endregion

        #region Camera Settings
        public const float CAMERA_HEIGHT = 10f;
        public const float CAMERA_ORTHOGRAPHIC_SIZE = 6f;
        #endregion
    }
}
