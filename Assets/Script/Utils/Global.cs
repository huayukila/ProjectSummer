using UnityEngine;
public static class Global 
{
    public static Vector3 PLAYER1_START_POSITION = new Vector3(0.0f, 0.64f, -5.0f);
    public static Vector3 PLAYER2_START_POSITION = new Vector3(0.0f, 0.64f, 15.0f);

    public static float SPEED_DOWN_COEFFICIENT = 0.5f;
    public static float SPEED_UP_COEFFICIENT = 1.25f;


    public static float DROP_POINT_INTERVAL = 0.1f;
    public static float DROP_POINT_ALIVE_TIME = 2.0f;

    public static float RESPAWN_TIME = 5.0f;
}
