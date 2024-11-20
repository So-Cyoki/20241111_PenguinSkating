using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState
{
    ICE, WATER, CATCH,
}

public abstract class ItemBase : MonoBehaviour
{
    [HideInInspector] public Rigidbody _rb;
    protected NWH.DWP2.WaterObjects.WaterObject _waterObject;//水插件脚本
    protected Animator _animator;

    float _submergedVolume;//浮力

    public ItemState _itemState = ItemState.WATER;

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

    public void SetState(ItemState state)
    {
        _itemState = state;
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ice") && _itemState != ItemState.CATCH)
        {
            _itemState = ItemState.ICE;
        }
    }
}
