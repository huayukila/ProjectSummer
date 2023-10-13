using UnityEngine;

public static class Global
{
    public static readonly Vector3 PLAYER1_START_POSITION = new Vector3(-80.0f, 0.64f, 0.0f);    // �v���C���[�P�̏����ʒu
    public static readonly Vector3 PLAYER2_START_POSITION = new Vector3( 80.0f, 0.64f, 0.0f);    // �v���C���[�Q�̏����ʒu

    public static readonly float SPEED_DOWN_COEFFICIENT = 0.7f;                                  // ���̃v���C���[�̗̈��ɂ��鎞�̃X�r�[�g�W��
    public static readonly float SPEED_UP_COEFFICIENT = 1.4f;                                   // �����̃v���C���[�̗̈��ɂ��鎞�̃X�r�[�g�W��

    public static readonly float DROP_POINT_INTERVAL = 0.1f;                                     // DropPoint���C���X�^���X�����鎞�ԊԊu
    public static readonly float DROP_POINT_ALIVE_TIME = 2.0f;                                   // DropPoint�̑��݂��Ă��鎞��

    public static readonly float RESPAWN_TIME = 5.0f;                                            // �v���C���[������ł��畜���܂ł̎��ԊԊu

    public static readonly Color PLAYER_ONE_TRACE_COLOR = new Color(0.0f, 0.0f, 1.0f, 1.0f);          // �v���C���[�P�̍��Ղ̐F
    public static readonly Color PLAYER_TWO_TRACE_COLOR = new Color(0.5f, 0.0f, 0.5f, 1.0f);          // �v���C���[�Q�̍��Ղ̐F

    public static readonly float PLAYER_MAX_MOVE_SPEED = 2000.0f;                                // �v���C���[�̍ő呬�x
    public static readonly float PLAYER_ACCELERATION = 800.0f;                                   // �v���C���[�̉����x
    public static readonly float PLAYER_ROTATION_SPEED = 3.5f;                                   // �v���C���[�̉�]���x

    public static readonly float STAGE_LENGTH = 174.0f;
    public static readonly float STAGE_WIDTH = 72.0f;

    public static readonly int SILK_SCORE = 100;

    public static readonly float SET_GAME_TIME = 60f;�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//�Q�[������

}
