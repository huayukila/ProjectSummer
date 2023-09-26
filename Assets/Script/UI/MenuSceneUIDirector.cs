using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneUIDirector : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))　　　　　　　　　　//GamingSceneへ切り替え
        {
            TypeEventSystem.Instance.Send<GamingSceneSwitch>();
        }
    }
}
