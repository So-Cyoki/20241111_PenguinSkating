using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_fish : ItemBase
{
    ItemState _preItemState;//前一帧的状态
    public Vector2 _speedForce;
    public Vector2 _idleTime;
    [Tooltip("输入角度")] public Vector3 _angleRun;//随机转多少角度移动

    protected override void Update()
    {
        base.Update();

        if (_itemState != _preItemState)
        {
            switch (_itemState)
            {
                case ItemState.ICE:
                case ItemState.WATER:
                    _animator.SetTrigger("tIdle");
                    break;
                case ItemState.CATCH:
                    _animator.SetTrigger("tCatch");
                    break;
            }
            _preItemState = _itemState;
        }
    }
}
