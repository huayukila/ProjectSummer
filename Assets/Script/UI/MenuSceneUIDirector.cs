using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneUIDirector : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown)�@�@�@�@�@�@�@�@�@�@//GamingScene�֐؂�ւ�
        {
            TypeEventSystem.Instance.Send<GamingSceneSwitch>();
        }
    }
}
