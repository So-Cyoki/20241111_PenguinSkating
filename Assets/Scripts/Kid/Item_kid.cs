using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_kid : ItemBase
{
    public SpriteRenderer _spriteRen;

    public Vector2 _speedForce;
    [Tooltip("输入角度")] public Vector3 _angleRun;//随机转多少角度移动
    public Vector2 _idleTime;
    public float _sitDownTime;
    public Vector2 _hungerTime;
    ItemState _preState;

    protected override void Update()
    {
        base.Update();

        if (_preState != GetState())
        {
            if (GetState() == ItemState.WATER)
            {
                _animator.SetTrigger("tDropWater");
            }
            _preState = GetState();
        }
    }
}
