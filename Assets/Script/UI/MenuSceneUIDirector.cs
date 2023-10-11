using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuSceneUIDirector : MonoBehaviour
{
    private InputAction _anyKeyAction;
    public InputActionAsset _anyValueAction;
    private void Awake()
    {
        _anyKeyAction = _anyValueAction.FindActionMap("AnyKey").FindAction("AnyKey");
    }
    void Update()
    {
        if (_anyKeyAction.triggered)　　　　　　　　　//GamingSceneへ切り替え
        {
            TypeEventSystem.Instance.Send<GamingSceneSwitch>();
        }
    }
    private void OnEnable() => _anyKeyAction.Enable();
    private void OnDisable() => _anyKeyAction.Disable();
}
