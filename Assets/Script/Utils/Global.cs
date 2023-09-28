using UnityEngine;

public static class Global
{
    public static readonly Vector3 PLAYER1_START_POSITION = new Vector3(0.0f, 0.64f, -40.0f);    // �v���C���[�P�̏����ʒu
    public static readonly Vector3 PLAYER2_START_POSITION = new Vector3(0.0f, 0.64f, 37.0f);     // �v���C���[�Q�̏����ʒu

    public static readonly float SPEED_DOWN_COEFFICIENT = 0.5f;                                  // ���̃v���C���[�̗̈��ɂ��鎞�̃X�r�[�g�W��
    public static readonly float SPEED_UP_COEFFICIENT = 1.25f;                                   // �����̃v���C���[�̗̈��ɂ��鎞�̃X�r�[�g�W��

    public static readonly float DROP_POINT_INTERVAL = 0.1f;                                     // DropPoint���C���X�^���X�����鎞�ԊԊu
    public static readonly float DROP_POINT_ALIVE_TIME = 2.0f;                                   // DropPoint�̑��݂��Ă��鎞��

    public static readonly float RESPAWN_TIME = 5.0f;                                            // �v���C���[������ł��畜���܂ł̎��ԊԊu

    public static readonly Color32 PLAYER_ONE_AREA_COLOR = new Color32(0, 0, 255, 255);          // �v���C���[�P�̍��Ղ̐F
    public static readonly Color32 PLAYER_TWO_AREA_COLOR = new Color32(0, 255, 0, 255);          // �v���C���[�Q�̍��Ղ̐F

    public static readonly float PLAYER_MAX_MOVE_SPEED = 800.0f;                                 // �v���C���[�̍ő呬�x
    public static readonly float PLAYER_ACCELERATION = 500.0f;                                   // �v���C���[�̉����x
    public static readonly float PLAYER_ROTATION_SPEED = 3.0f;                                   // �v���C���[�̉�]���x

}
