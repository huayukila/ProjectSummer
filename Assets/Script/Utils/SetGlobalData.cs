using UnityEngine;

[CreateAssetMenu(menuName = "Global/Create GlobalData")]
public class SetGlobalData : ScriptableObject
{
    public Vector3 player1
    {
        get
        {
            return Global.PLAYER1_START_POSITION;
        }

        set
        {
            Global.PLAYER1_START_POSITION = value;
        }
    }
    public Vector3 player2
    {
        get
        {
            return Global.PLAYER2_START_POSITION;
        }

        set
        {
            Global.PLAYER2_START_POSITION = value;
        }
    }
    public float speedDownCoefficient
    {
        get
        {
            return Global.SPEED_DOWN_COEFFICIENT;
        }

        set
        {
            Global.SPEED_DOWN_COEFFICIENT = value;
        }
    }
    public float speedUpCoefficient
    {
        get
        {
            return Global.SPEED_UP_COEFFICIENT;
        }

        set
        {
            Global.SPEED_UP_COEFFICIENT = value;
        }
    }
    public float dropPointInterval
    {
        get
        {
            return Global.DROP_POINT_INTERVAL;
        }

        set
        {
            Global.DROP_POINT_INTERVAL = value;
        }
    }
    public float dropPointAliveTime
    {
        get
        {
            return Global.DROP_POINT_ALIVE_TIME;
        }

        set
        {
            Global.DROP_POINT_ALIVE_TIME = value;
        }
    }
    public float respawnTime
    {
        get
        {
            return Global.RESPAWN_TIME;
        }

        set
        {
            Global.RESPAWN_TIME = value;
        }
    }
    public Color32 playerOneAreaColor
    {
        get
        {
            return Global.PLAYER_ONE_AREA_COLOR;
        }

        set
        {
            Global.PLAYER_ONE_AREA_COLOR = value;
        }
    }
    public Color32 playerTwoAreaColor
    {
        get
        {
            return Global.PLAYER_TWO_AREA_COLOR;
        }

        set
        {
            Global.PLAYER_TWO_AREA_COLOR = value;
        }
    }
    public float playerMaxMoveSpeed
    {
        get
        {
            return Global.PLAYER_MAX_MOVE_SPEED;
        }

        set
        {
            Global.PLAYER_MAX_MOVE_SPEED = value;
        }
    }
    public float playerAcceleration
    {
        get
        {
            return Global.PLAYER_ACCELERATION;
        }

        set
        {
            Global.PLAYER_ACCELERATION = value;
        }
    }
    public float playerRotationSpeed
    {
        get
        {
            return Global.PLAYER_ROTATION_SPEED;
        }

        set
        {
            Global.PLAYER_ROTATION_SPEED = value;
        }
    }

}
