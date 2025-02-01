using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_fish : ItemBase
{
    public ParticleSystem _deadPar;
    public ParticleSystem _runPar;
    public Vector2 _speedForce;
    public Vector2 _idleTime;
    [Tooltip("输入角度")] public Vector3 _angleRun;//随机转多少角度移动

    bool _isDead;

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (_isDead)
        {
            //死亡的粒子效果结束后删除对象
            if (!_deadPar.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
    public void ToDead()
    {
        _deadPar.Play();
        _coll.enabled = false;
        _spriteRen.enabled = false;
        _meshRen.enabled = false;
        _rb.detectCollisions = false;
        _rb.isKinematic = true;
        _isDead = true;
    }
}
