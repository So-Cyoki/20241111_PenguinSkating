using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerManager : MonoBehaviour
{
    public PlayerInput player1Input;
    public PlayerInput player2Input;

    private void Start()
    {
        // 获取所有已连接的 Gamepad 设备
        var gamepads = Gamepad.all;

        // 为 Player1 分配第一个手柄
        if (gamepads.Count >= 1)
        {
            player1Input.SwitchCurrentControlScheme("Gamepad1", gamepads[0]);
            Debug.Log("Player1 绑定到手柄: " + gamepads[0].name);
        }

        // 为 Player2 分配第二个手柄
        if (gamepads.Count >= 2)
        {
            player2Input.SwitchCurrentControlScheme("Gamepad2", gamepads[1]);
            Debug.Log("Player2 绑定到手柄: " + gamepads[1].name);
        }
    }
}
