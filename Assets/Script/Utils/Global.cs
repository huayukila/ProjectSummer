using UnityEngine;

public static class Global
{
    public static Vector3 PLAYER1_START_POSITION = new Vector3(0.0f, 0.64f, -40.0f);    // プレイヤー１の初期位置
    public static Vector3 PLAYER2_START_POSITION = new Vector3(0.0f, 0.64f, 37.0f);     // プレイヤー２の初期位置

    public static float SPEED_DOWN_COEFFICIENT = 0.5f;                                  // 他のプレイヤーの領域上にいる時のスビート係数
    public static float SPEED_UP_COEFFICIENT = 1.25f;                                   // 自分のプレイヤーの領域上にいる時のスビート係数

    public static float DROP_POINT_INTERVAL = 0.1f;                                     // DropPointをインスタンス化する時間間隔
    public static float DROP_POINT_ALIVE_TIME = 2.0f;                                   // DropPointの存在している時間

    public static float RESPAWN_TIME = 5.0f;                                            // プレイヤーが死んでから復活までの時間間隔

    public static Color32 PLAYER_ONE_AREA_COLOR = new Color32(255, 0, 0, 255);          // プレイヤー１の痕跡の色
    public static Color32 PLAYER_TWO_AREA_COLOR = new Color32(0, 255, 0, 255);          // プレイヤー２の痕跡の色

    public static float PLAYER_MAX_MOVE_SPEED = 800.0f;                                 // プレイヤーの最大速度
    public static float PLAYER_ACCELERATION = 500.0f;                                   // プレイヤーの加速度
    public static float PLAYER_ROTATION_SPEED = 3.0f;                                   // プレイヤーの回転速度

}
