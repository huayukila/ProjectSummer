using UnityEngine;

public static class Global
{
    #region Player
    public struct PowerUp
    {
        public float SpeedUp;
        public float RotateUp;
    }
    // public static readonly Vector3[] PLAYER_START_POSITIONS =
    // {
    //     new(-85.0f, 0.64f, 0.0f),

    //     new(85.0f, 0.64f, 0.0f)
    // };

    public static readonly Vector3[] PLAYER_DEFAULT_FORWARD =
    {
        Vector3.right,
        Vector3.left,
        Vector3.back,
        Vector3.forward
    };

    public static readonly Color[] PLAYER_TRACE_COLORS =
    {
        new Color(0.878f, 0.114f, 0.886f, 1.0f),
        new Color(0.267f, 0.541f, 0.792f, 1.0f)
    };
    public static readonly float RESPAWN_TIME = 5.0f; // プレイヤーが死んでから復活までの時間間隔
    #region PlayerStatus
    public static readonly float SPEED_DOWN_COEFFICIENT = 0.8f; // 他のプレイヤーの領域上にいる時のスビート係数
    public static readonly float SPEED_UP_COEFFICIENT = 1.25f; // 自分のプレイヤーの領域上にいる時のスビート係数

    public static readonly float PLAYER_MAX_MOVE_SPEED = 48.0f; // プレイヤーの最大速度
    public static readonly float PLAYER_ACCELERATION = 15.0f; // プレイヤーの加速度
    public static readonly float PLAYER_ROTATION_SPEED = 4.0f; // プレイヤーの回転速度

    public static readonly float BOOST_DURATION_TIME = 1.0f;
    public static readonly float BOOST_COOLDOWN_TIME = 6.0f;

    public static readonly PowerUp[] POWER_UP_PARAMETER =
    {
        new PowerUp() { SpeedUp=6f,RotateUp=-0.5f },
        new PowerUp() { SpeedUp=12f,RotateUp=-0.8f },
        new PowerUp() { SpeedUp=18f,RotateUp=-1.2f }
    };

    #endregion
    #endregion

    #region DropPoint
    public static readonly float DROP_POINT_INTERVAL = 0.5f; // DropPointをインスタンス化する時間間隔
    public static readonly float DROP_POINT_ALIVE_TIME = 2.0f; // DropPointの存在している時間
    #endregion

    #region Silk
    public static readonly int SILK_SCORE = 100;
    public static readonly int MAX_SILK_COUNT = 3;
    public static readonly float SILK_SPAWN_TIME = 6.0f;
    #endregion

    #region Item
    public static readonly Vector3 ITEM_BOX_POS = new Vector3 (0f,0.64f,0f);
    public static readonly float ITEM_BOX_SPAWN_TIME = 10f;
    public static readonly float ON_SLIP_TIME = 1f;
    public static readonly float ON_STUN_TIME = 2f;
    public static readonly float ON_SLIP_MIN_SPEED = 8f;
    public static readonly float STUN_SILK_SPEED = (PLAYER_MAX_MOVE_SPEED + POWER_UP_PARAMETER[POWER_UP_PARAMETER.Length - 1].SpeedUp) * 1.3f;
    public static readonly float BUBBLE_EXPLODE_TIME = 3f;
    #endregion

    public static readonly float SET_GAME_TIME = 180f; //ゲーム時間

    public static readonly float INTSTRUCTON_SCENE_TIME = 1.5f; //ゲーム操作方法紹介画面 PRESS無効の時間
    public static readonly float CREDITS_SCENE_TIME = 1.5f; //ゲーム終了後の感謝画面 PRESS無効の時間

    public static readonly Vector3 GAMEOBJECT_STACK_POS = new Vector3(10000f, 0, 0); //生成されたオブジェクトの保存座標

    #region Map

    public static readonly int MAP_SIZE_WIDTH = 2;//マップの長さ
    public static readonly int MAP_SIZE_HEIGHT = 2;//マップの高さ
    public static readonly float STAGE_WIDTH = MAP_SIZE_WIDTH * 100f;
    public static readonly float STAGE_HEIGHT = MAP_SIZE_HEIGHT * 100f;
    #endregion

    #region SkillUI
    public static readonly float BOOST_BAR_CHARGING_SPEED = 0.05f;
    #endregion
}