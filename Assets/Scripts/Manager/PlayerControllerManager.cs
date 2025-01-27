using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerManager : MonoBehaviour
{
    public GameObject player1Object; // 玩家1对象
    public GameObject player2Object; // 玩家2对象

    Dictionary<int, PlayerInput> playerBindings = new(); // 玩家绑定
    Dictionary<int, Gamepad> gamepadBindings = new(); // 手柄绑定

    void Start()
    {
        // 初始化玩家绑定
        playerBindings[1] = player1Object.GetComponent<PlayerInput>();
        playerBindings[2] = player2Object.GetComponent<PlayerInput>();

        // 初始化手柄绑定为null
        gamepadBindings[1] = null;
        gamepadBindings[2] = null;

        // 手动检查已连接的手柄
        UpdateGamepadBindings();
    }

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange; // 监听设备连接和断开
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad gamepad)
        {
            if (change == InputDeviceChange.Added)
            {
                Debug.Log($"手柄连接：{gamepad.name}");
                AssignGamepad(gamepad); // 绑定手柄到玩家
            }
            else if (change == InputDeviceChange.Removed)
            {
                Debug.Log($"手柄断开：{gamepad.name}");
                UnassignGamepad(gamepad); // 解绑手柄
            }
        }
    }

    // 将手柄绑定到固定的玩家
    private void AssignGamepad(Gamepad gamepad)
    {
        for (int playerIndex = 1; playerIndex <= 2; playerIndex++)
        {
            if (gamepadBindings[playerIndex] == null)
            {
                gamepadBindings[playerIndex] = gamepad; // 绑定手柄到玩家
                playerBindings[playerIndex].SwitchCurrentControlScheme("Gamepad", new InputDevice[] { gamepad });
                Debug.Log($"手柄 {gamepad.name} 绑定到 Player{playerIndex}");
                return;
            }
        }

        Debug.Log($"手柄 {gamepad.name} 没有空闲的玩家可绑定");
    }

    // 解绑手柄
    private void UnassignGamepad(Gamepad gamepad)
    {
        for (int playerIndex = 1; playerIndex <= 2; playerIndex++)
        {
            if (gamepadBindings[playerIndex] == gamepad)
            {
                gamepadBindings[playerIndex] = null; // 解绑手柄
                playerBindings[playerIndex].SwitchCurrentControlScheme("None", null); // 移除绑定
                Debug.Log($"手柄 {gamepad.name} 从 Player{playerIndex} 解绑");
                return;
            }
        }
    }

    // 手动检查已连接的手柄并更新绑定
    private void UpdateGamepadBindings()
    {
        var gamepads = Gamepad.all;
        foreach (var gamepad in gamepads)
        {
            AssignGamepad(gamepad);
        }
    }
}
