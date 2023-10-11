using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchSceneInputManager : MonoBehaviour
{
    [SerializeField]
    private InputAction _anyKeyAction;
    [SerializeField]
    private InputActionAsset _anyValueAction;
    private void Awake()
    {
        _anyKeyAction = _anyValueAction.FindActionMap("AnyKey").FindAction("AnyKey");
    }

    // Update is called once per frame
    void Update()
    {
        
        if(_anyKeyAction.triggered)
        {
            Debug.Log("Any Button");
        }
        
    }

    private void OnEnable() => _anyKeyAction.Enable();
    private void OnDisable() => _anyKeyAction.Disable();

}
