using System;
using UnityEngine;

public class IceMountainMap : MonoBehaviour
{
    //用于下一个地图块的连接点
    public Transform _endPoint;
    public BoxCollider _collider1;//中间碰撞体
    public BoxCollider _collider2;//结尾碰撞体

    public static event Action<Vector3> OnMoveMiddle;//触发了中间点的碰撞
    public static event Action OnMoveLast;//触发了结尾点的碰撞

    public void GameRestart()
    {
        _collider1.enabled = true;
        _collider2.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && _collider1.enabled)
        {
            OnMoveMiddle?.Invoke(_endPoint.position);
            _collider1.enabled = false;
        }
        else if (other.gameObject.CompareTag("Player") && _collider2.enabled)
        {
            OnMoveLast?.Invoke();
            _collider2.enabled = false;
        }
    }

    private void OnEnable()
    {
        SeaWave.OnGameOver += GameRestart;
    }
    private void OnDisable()
    {
        SeaWave.OnGameOver -= GameRestart;
    }
}
