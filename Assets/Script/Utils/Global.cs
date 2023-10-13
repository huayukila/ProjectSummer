using UnityEngine;

public static class Global
{
    public static readonly Vector3 PLAYER1_START_POSITION = new Vector3(-80.0f, 0.64f, 0.0f);    // プレイヤー１の初期位置
    public static readonly Vector3 PLAYER2_START_POSITION = new Vector3( 80.0f, 0.64f, 0.0f);    // プレイヤー２の初期位置

    public static readonly float SPEED_DOWN_COEFFICIENT = 0.7f;                                  // 他のプレイヤーの領域上にいる時のスビート係数
    public static readonly float SPEED_UP_COEFFICIENT = 1.4f;                                   // 自分のプレイヤーの領域上にいる時のスビート係数

    public static readonly float DROP_POINT_INTERVAL = 0.1f;                                     // DropPointをインスタンス化する時間間隔
    public static readonly float DROP_POINT_ALIVE_TIME = 2.0f;                                   // DropPointの存在している時間

    public static readonly float RESPAWN_TIME = 5.0f;                                            // プレイヤーが死んでから復活までの時間間隔

    public static readonly Color PLAYER_ONE_TRACE_COLOR = new Color(0.0f, 0.0f, 1.0f, 1.0f);          // プレイヤー１の痕跡の色
    public static readonly Color PLAYER_TWO_TRACE_COLOR = new Color(0.5f, 0.0f, 0.5f, 1.0f);          // プレイヤー２の痕跡の色

    public static readonly float PLAYER_MAX_MOVE_SPEED = 2000.0f;                                // プレイヤーの最大速度
    public static readonly float PLAYER_ACCELERATION = 800.0f;                                   // プレイヤーの加速度
    public static readonly float PLAYER_ROTATION_SPEED = 3.5f;                                   // プレイヤーの回転速度

    public static readonly float STAGE_LENGTH = 174.0f;
    public static readonly float STAGE_WIDTH = 72.0f;

    public static readonly int SILK_SCORE = 100;

    public static readonly float SET_GAME_TIME = 60f;　　　　　　　　　　　　　　　　　　　　　　　//ゲーム時間

}
