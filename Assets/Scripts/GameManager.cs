using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    PlayerInputActions _inputActions;

    private void Awake()
    {
        _inputActions = new();
    }
    private void LateUpdate()
    {
        if (_inputActions.UI.Exit.WasPressedThisFrame())
        {
            Application.Quit();// 退出应用程序
        }
    }
    private void OnEnable()
    {
        _inputActions.Enable();
    }
    private void OnDisable()
    {
        _inputActions.Disable();
    }
}
