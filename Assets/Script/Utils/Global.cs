using UnityEngine;

public static class Global
{
    #region Player
    
    internal struct PowerUp
    {
        public float SpeedUp;
        public float RotateUp;
    }

    public static int PLAYER_MAX_COUNT = 2;

    public static readonly Vector3[] PLAYER_START_POSITIONS =
    {
        new(-85.0f, 0.64f, 0.0f),

        new(85.0f, 0.64f, 0.0f)
    };

    public static readonly Vector3[] PLAYER_DEFAULT_FORWARD =
    {
        Vector3.right,
        Vector3.left,
        Vector3.back,
        Vector3.forward
    };

    public static readonly Color[] PLAYER_TRACE_COLORS =
    {
        // new Color(0.878f, 0.114f, 0.886f, 1.0f),
        // new Color(0.267f, 0.541f, 0.792f, 1.0f)
        new Color(194/255f, 28/255f, 118/255f, 1),
        new Color(0, 152/255f, 253/255f, 1)
    };

    public static readonly float RESPAWN_TIME = 5.0f; // ?v???C???[?????????’{??????????u

    #region PlayerStatus

    public static readonly float SPEED_DOWN_COEFFICIENT = 0.8f; // ????v???C???[???????????X?r?[?g?W??
    public static readonly float SPEED_UP_COEFFICIENT = 1.25f; // ??????v???C???[???????????X?r?[?g?W??

    public static readonly float PLAYER_MAX_MOVE_SPEED = 48.0f; // ?v???C???[?????x
    public static readonly float PLAYER_ACCELERATION = 15.0f; // ?v???C???[??????x
    public static readonly float PLAYER_ROTATION_SPEED = 4.0f; // ?v???C???[???]???x

    public static readonly float BOOST_DURATION_TIME = 1.0f;
    public static readonly float BOOST_COOLDOWN_TIME = 6.0f;

    internal static readonly PowerUp[] POWER_UP_PARAMETER =
    {
        new PowerUp() { SpeedUp = 6f, RotateUp = -0.5f },
        new PowerUp() { SpeedUp = 12f, RotateUp = -0.8f },
        new PowerUp() { SpeedUp = 18f, RotateUp = -1.2f }
    };

    #endregion

    #endregion

    #region DropPoint

    public static readonly float DROP_POINT_INTERVAL = 0.05f; // DropPoint???C???X?^???X?????????u
    public static readonly float DROP_POINT_ALIVE_TIME = 2.0f; // DropPoint?????????????

    #endregion

    #region Silk

    public static readonly int SILK_SCORE = 100;
    public static readonly int MAX_SILK_COUNT = 3;
    public static readonly float SILK_SPAWN_TIME = 6.0f;

    #endregion

    #region Item

    public static readonly Vector3 ITEM_BOX_POS = new Vector3(0f, 0.64f, 0f);
    public static readonly float ITEM_BOX_SPAWN_TIME = 10f;
    public static readonly float ON_SLIP_TIME = 1f;
    public static readonly float ON_STUN_TIME = 2f;
    public static readonly float ON_SLIP_MIN_SPEED = 8f;

    public static readonly float STUN_SILK_SPEED =
        (PLAYER_MAX_MOVE_SPEED + POWER_UP_PARAMETER[POWER_UP_PARAMETER.Length - 1].SpeedUp) * 1.3f;

    public static readonly float BUBBLE_EXPLODE_TIME = 3f;

    #endregion

    public static readonly float SET_GAME_TIME = 180f; //?Q?[??????

    public static readonly float INTSTRUCTON_SCENE_TIME = 1.5f; //?Q?[????????@????? PRESS?????????
    public static readonly float CREDITS_SCENE_TIME = 1.5f; //?Q?[???I?????????? PRESS?????????

    public static readonly Vector3 GAMEOBJECT_STACK_POS = new Vector3(10000f, 0, 0); //?????????I?u?W?F?N?g???????W

    #region Map
    
    public static readonly float MAP_SIZE_WIDTH = 3; //?}?b?v?????
    public static readonly float MAP_SIZE_HEIGHT = 1.6875f; //?}?b?v?????

    public static readonly float STAGE_WIDTH = MAP_SIZE_WIDTH * 100f;
    public static readonly float STAGE_HEIGHT = MAP_SIZE_HEIGHT * 100f;

    #endregion

    #region SkillUI

    public static readonly float BOOST_BAR_CHARGING_SPEED = 0.05f;

    #endregion
}