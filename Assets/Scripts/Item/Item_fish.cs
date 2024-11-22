using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_fish : ItemBase
{
    public Vector2 _speedForce;
    public Vector2 _idleTime;
    [Tooltip("输入角度")] public Vector3 _angleRun;//随机转多少角度移动
}
