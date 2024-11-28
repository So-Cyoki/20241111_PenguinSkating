using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState
{
    ORIGINAL, ICE, WATER, CATCH
}
//物体本身有3个状态：冰块上、水上、被抓住
//这3个状态是高于状态机的，所以可以通过这3个状态进行判断，做一些特殊的逻辑
//而且每个物体上还有状态机，是使用动画系统制作的
//注意：状态机上需要有isCatch，因为这个是用来处理被抓起和被放下的逻辑的

public abstract class ItemBase : MonoBehaviour
{
    [HideInInspector] public Rigidbody _rb;
    protected NWH.DWP2.WaterObjects.WaterObject _waterObject;//水插件脚本
    protected Animator _animator;

    public Transform _playerTrans;
    public float _destoryLength = 300;//销毁距离
    [Tooltip("增加质量倍率")] public float _addMassM = 1;
    public float _originalMass;
    readonly float _checkWaterTime = 0.3f;//多久检查一次是否水状态
    float _currentCheckWaterTime = 0;
    float _submergedVolume;//浮力

    [SerializeField] ItemState _itemState = ItemState.ORIGINAL;
    ItemState _currentItemState = ItemState.ORIGINAL;
    public ItemState GetState() { return _itemState; }

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _originalMass = _rb.mass;
        _waterObject = GetComponent<NWH.DWP2.WaterObjects.WaterObject>();
        _animator = GetComponent<Animator>();
    }
    protected virtual void Start() { }
    protected virtual void Update()
    {
        //检查是否需要超过距离需要销毁
        if (_playerTrans != null && (_playerTrans.position - transform.position).sqrMagnitude >= _destoryLength * _destoryLength)
        {
            Destroy(gameObject);
        }
        if (_currentItemState != _itemState)
        {
            //重量变化机制(为了在冰上的时候更重)
            if (!CompareTag("Ice"))//冰块就不需要改变重量了
            {
                switch (_itemState)
                {
                    case ItemState.ORIGINAL:
                    case ItemState.CATCH:
                        _rb.mass = _originalMass;
                        break;
                    case ItemState.ICE:
                        _rb.mass *= _addMassM;
                        break;
                }
            }
            _currentItemState = _itemState;
        }
    }

    protected virtual void FixedUpdate()
    {
        _submergedVolume = _waterObject.submergedVolume;//水浮力
        //是否进入水中判断(为了减少进入水状态的误差，加入了时间机制，过一段时间才检查一次)
        if (_itemState == ItemState.ORIGINAL)
        {
            if (_submergedVolume > 1f && _rb.velocity != Vector3.zero)
            {
                _currentCheckWaterTime += Time.deltaTime;
                if (_currentCheckWaterTime >= _checkWaterTime)
                {
                    _itemState = ItemState.WATER;
                }
            }
        }
        //退出水状态
        if (_itemState == ItemState.WATER)
        {
            if (_submergedVolume <= 0f)
                _itemState = ItemState.ORIGINAL;
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="pos">实例化坐标</param>
    /// <param name="player">玩家的坐标</param>
    public virtual void Initial(Vector3 pos, Transform player)
    {
        _itemState = ItemState.ORIGINAL;
        transform.position = pos;
        _playerTrans = player;

        _currentCheckWaterTime = 0;
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
            _animator.SetBool("isCatch", true);
            _currentCheckWaterTime = 0;
        }
        //被放下
        else
        {
            _itemState = ItemState.ORIGINAL;
            _animator.SetBool("isCatch", false);
        }
    }
    protected virtual void OnCollisionEnter(Collision other)
    {
        //是否在冰块上判断
        if (_itemState == ItemState.ORIGINAL)
            if (other.gameObject.CompareTag("Ice")
            || other.gameObject.CompareTag("IceMountain"))
            {
                _itemState = ItemState.ICE;
                _currentCheckWaterTime = 0;
            }
    }
    protected virtual void OnCollisionStay(Collision other) { }
    protected virtual void OnCollisionExit(Collision other)
    {
        //是否在冰块上判断
        if (_itemState == ItemState.ICE)
            if (other.gameObject.CompareTag("Ice")
            || other.gameObject.CompareTag("IceMountain"))
            {
                _itemState = ItemState.ORIGINAL;
            }
    }
}
