using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    protected override void Awake()
    {
        
    }

    private void Start()
    {
        for(int i = 0; i < PlayerInput.all.Count; ++i)
        {
            PlayerInput.all[i].SwitchCurrentControlScheme(
            "Keyboard&Mouse",
            Keyboard.current);
            PlayerInput.all[i].neverAutoSwitchControlSchemes = true;
        }
        for(int i = 0; i < Gamepad.all.Count;++i)
        {
            PlayerInput.all[i].SwitchCurrentControlScheme(
            "Gamepad",
            Gamepad.all[i]);
        }
    }
}
