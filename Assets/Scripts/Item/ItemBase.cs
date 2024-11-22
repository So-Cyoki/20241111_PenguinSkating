using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState
{
    ICE, WATER, CATCH
}
//物体本身有3个状态：冰块上、水上、被抓住
//这3个状态是高于状态机的，所以可以通过这3个状态进行判断，做一些特殊的逻辑
//而且每个物体上还有状态机，是使用动画系统制作的
//注意：状态机上需要有tCatch和tExitCatch，因为这个是用来处理被抓起和被放下的逻辑的

public abstract class ItemBase : MonoBehaviour
{
    [HideInInspector] public Rigidbody _rb;
    protected NWH.DWP2.WaterObjects.WaterObject _waterObject;//水插件脚本
    protected Animator _animator;

    float _submergedVolume;//浮力

    ItemState _itemState = ItemState.WATER;
    public ItemState GetState() { return _itemState; }

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _waterObject = GetComponent<NWH.DWP2.WaterObjects.WaterObject>();
        _animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {
        //是否进入水中判断
        if (_itemState != ItemState.CATCH && _itemState != ItemState.WATER)
        {
            _submergedVolume = _waterObject.submergedVolume;
            if (_submergedVolume > 1f)
                _itemState = ItemState.WATER;
        }
    }

    /// <summary>
    /// 处理被抓起和放下的状态变换
    /// </summary>
    /// <param name="flag">true:抓起/false:放下</param>
    public virtual void SetCatch(bool flag)
    {
        //被抓起
        if (flag)
        {
            _itemState = ItemState.CATCH;
            _animator.SetTrigger("tCatch");
        }
        //被放下
        else
        {
            _itemState = ItemState.WATER;
            _animator.SetTrigger("tExitCatch");
        }
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        //是否在冰块上判断
        if (_itemState != ItemState.CATCH && other.gameObject.CompareTag("Ice"))
        {
            _itemState = ItemState.ICE;
        }
    }
}
