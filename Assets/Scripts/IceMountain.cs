using System;
using UnityEngine;

public class IceMountain : MonoBehaviour
{
    //用于下一个地图块的连接点
    public Transform _endPoint;
    //用于提供给父物体的碰撞位置，开始会将位置和大小信息覆盖给父物体的Collider
    public Transform _colliderGrid;
    //父物体上的触发器，用于判断玩家是否经过地图块一半了
    BoxCollider _collider;

    public static event Action<Vector3, GameObject> OnMoveMiddle;//触发了中间点的碰撞

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }
    private void Start()
    {
        //将用于展示用的方块的位置和大小赋值给父物体的碰撞体
        _collider.center = _colliderGrid.localPosition;
        _collider.size = _colliderGrid.localScale;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnMoveMiddle?.Invoke(_endPoint.position, gameObject);
        }
    }
}
