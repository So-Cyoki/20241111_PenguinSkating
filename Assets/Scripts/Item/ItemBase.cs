using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState
{
    ICE, WATER, CATCH
}

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
        if (_itemState != ItemState.CATCH && other.gameObject.CompareTag("Ice"))
        {
            _itemState = ItemState.ICE;
        }
    }
}
