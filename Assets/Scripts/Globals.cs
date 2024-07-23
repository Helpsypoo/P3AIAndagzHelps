using UnityEngine;

public static class Globals {

    #region Tags
    public const string WALKABLE_TAG = "Walkable";
    public const string NOT_WALKABLE_TAG = "NotWalkable";
    public const string UNIT_TAG = "Unit";
    public const string ENEMY_TAG = "Enemy";
    public const string DOWNED_UNIT_TAG = "DownedUnit";
    public const string WAYPOINT_TAG = "Waypoint";
    public const string LIBERATED_TAG = "Liberated";
    #endregion

    #region Layers
    public const int ENTITIES_LAYER = 7;
    #endregion
    
    #region LayerMasks
    public static readonly LayerMask SELECTION_LAYERMASK = LayerMask.GetMask("Entities", "Default", "Hitbox");
    #endregion

    /// <summary>
    /// The minimum distance a unit has to be to perform an action on something.
    /// </summary>
    public const float MIN_ACTION_DIST = 1f;

    /// <summary>
    /// The distance at which one unit following another unit stands.
    /// </summary>
    public const float FOLLOW_DIST = 1.75f;

    public const float BULLET_SPEED = 4000f;

    public const float REVIVE_TIMER = 3f;


}
