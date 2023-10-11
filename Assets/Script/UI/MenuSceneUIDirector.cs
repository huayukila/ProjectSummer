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
        _anyKeyAction.performed += OnSwitchScene;
    }
    void Update()
    {

    }
    private void OnEnable() => _anyKeyAction.Enable();
    private void OnDisable() => _anyKeyAction.Disable();

    private void OnSwitchScene(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            TypeEventSystem.Instance.Send<GamingSceneSwitch>();
        }

    }

    private void OnDestroy() => _anyKeyAction.performed -= OnSwitchScene;
}
