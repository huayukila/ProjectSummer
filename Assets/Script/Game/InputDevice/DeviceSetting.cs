using UnityEngine.InputSystem;

public static class DeviceSetting
{
    private static int _gamepadCount = Gamepad.all.Count;   
    public static void Init()
    {
        _gamepadCount = Gamepad.all.Count;
        foreach (var playerinput in PlayerInput.all)
        {
            playerinput.SwitchCurrentControlScheme(
            "Keyboard&Mouse",
            Keyboard.current);
            playerinput.neverAutoSwitchControlSchemes = true;
        }
        for (int i = 0; i < _gamepadCount; ++i)
        {
            if (i >= PlayerInput.all.Count)
                break;
            PlayerInput.all[i].SwitchCurrentControlScheme(
            "Gamepad",
            Gamepad.all[i]);
        }


    }
}
