using UnityEngine;

public static class Global
{
    #region Player
    public struct PowerUp
    {
        public float SpeedUp;
        public float RotateUp;
    }
    public static readonly Vector3[] PLAYER_START_POSITIONS =
    {
        new(-80.0f, 0.64f, 0.0f),

        new(80.0f, 0.64f, 0.0f)
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
        new Color(0.878f, 0.114f, 0.886f, 1.0f),
        new Color(0.267f, 0.541f, 0.792f, 1.0f)
    };
    public static readonly float RESPAWN_TIME = 5.0f; // �v���C���[������ł��畜���܂ł̎��ԊԊu
    #region PlayerStatus
    public static readonly float SPEED_DOWN_COEFFICIENT = 0.5f; // ���̃v���C���[�̗̈��ɂ��鎞�̃X�r�[�g�W��
    public static readonly float SPEED_UP_COEFFICIENT = 1.25f; // �����̃v���C���[�̗̈��ɂ��鎞�̃X�r�[�g�W��

    public static readonly float PLAYER_MAX_MOVE_SPEED = 36.0f; // �v���C���[�̍ő呬�x
    public static readonly float PLAYER_ACCELERATION = 0.3f; // �v���C���[�̉����x
    public static readonly float PLAYER_ROTATION_SPEED = 3.0f; // �v���C���[�̉�]���x

    public static readonly float BOOST_DURATION_TIME = 1.0f;
    public static readonly float BOOST_COOLDOWN_TIME = 5.0f;

    public static readonly PowerUp[] POWER_UP_PARAMETER =
    {
        new PowerUp() { SpeedUp=6f,RotateUp=0.25f },
        new PowerUp() { SpeedUp=12f,RotateUp=0.5f },
        new PowerUp() { SpeedUp=18f,RotateUp=1f }
    };

    #endregion
    #endregion

    #region DropPoint
    public static readonly float DROP_POINT_INTERVAL = 0.05f; // DropPoint���C���X�^���X�����鎞�ԊԊu
    public static readonly float DROP_POINT_ALIVE_TIME = 1.5f; // DropPoint�̑��݂��Ă��鎞��
    #endregion

    #region Silk
    public static readonly int SILK_SCORE = 100;
    public static readonly int MAX_SILK_COUNT = 3;
    public static readonly float SILK_SPAWN_TIME = 6.0f;
    #endregion


    public static readonly float SET_GAME_TIME = 60f; //�Q�[������

    public static readonly float INTSTRUCTON_SCENE_TIME = 1.5f; //�Q�[��������@�Љ��� PRESS�����̎���
    public static readonly float CREDITS_SCENE_TIME = 1.5f; //�Q�[���I����̊��Ӊ�� PRESS�����̎���

    public static readonly Vector3 GAMEOBJECT_STACK_POS = new Vector3(0, 1000.0f, 0); //�������ꂽ�I�u�W�F�N�g�̕ۑ����W

    #region Map

    public static readonly int Map_Size_X = 2;
    public static readonly int Map_Size_Y = 2;
    public static readonly float STAGE_WIDTH = 100f;
    public static readonly float STAGE_HEIGHT = 100f;
    #endregion
}