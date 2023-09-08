using UnityEngine;
public static class Global 
{
    public const int iMAX_TAIL_COUNT = 50;      // êKîˆÇÃêﬂÇÃç≈ëÂå¬êî

    public static Vector3 PLAYER1_START_POSITION = new Vector3(0.0f, 0.64f, -5.0f);
    public static Vector3 PLAYER2_START_POSITION = new Vector3(0.0f, 0.64f, 15.0f);

    public static Color DEAD_COLOR = new Color(0.5f, 0.5f, 0.5f, 0.0f);

    public static Color PLAYER_ONE_TRAIL_COLOR = Color.blue;
    public static Color PLAYER_TWO_TRAIL_COLOR = Color.red;
    public static Color PLAYER_ONE_AREA_COLOR = Color.yellow;
    public static Color PLAYER_TWO_AREA_COLOR = Color.green;

    public static float SPEED_DOWN_COEFFICIENT = 0.5f;
    public static float SPEED_UP_COEFFICIENT = 1.5f;


    public static float DROP_POINT_INTERVAL = 0.1f;
    public static float DROP_POINT_ALIVE_TIME = 3.0f;

    public static float RESPAWN_TIME = 5.0f;
}
