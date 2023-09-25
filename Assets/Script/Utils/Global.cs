using UnityEngine;

public static class Global
{
    public static Vector3 PLAYER1_START_POSITION = new Vector3(0.0f, 0.64f, -40.0f);
    public static Vector3 PLAYER2_START_POSITION = new Vector3(0.0f, 0.64f, 37.0f);

    public static float SPEED_DOWN_COEFFICIENT = 0.5f;
    public static float SPEED_UP_COEFFICIENT = 1.25f;


    public static float DROP_POINT_INTERVAL = 0.1f;
    public static float DROP_POINT_ALIVE_TIME = 2.0f;

    public static float RESPAWN_TIME = 5.0f;

    public static Color32 PLAYER_ONE_AREA_COLOR = new Color32(0, 0, 255, 255);
    public static Color32 PLAYER_TWO_AREA_COLOR = new Color32(0, 255, 0, 255);

    public static float PLAYER_MAX_MOVE_SPEED = 500.0f;
    public static float PLAYER_ACCELERATION = 300.0f;
    public static float PLAYER_ROTATION_SPEED = 3.0f;

}
