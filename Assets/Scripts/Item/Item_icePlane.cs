using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_icePlane : ItemBase
{
    NWH.DWP2.WaterObjects.MassFromVolume _waterObjectMass;//用来自动计算出质量的脚本
    public Vector2 _startSize = new(5, 10);//随机的大小
    public float _minimizeValue;//单次减少的值
    [Tooltip("多久减少一次")] public float _uptakeTime;
    [Tooltip("Lerp:0~1")] public float _meltingLerp;//缩小的速率，使用Lerp
    float _currentUptakeTime;
    float _meltingSize;//融化后的大小
    float _size;//当前的大小(厚度不变)
    float _maxSmallSize = 0.5f;//最小大小(再小就会设置成死亡)

    [HideInInspector] public bool _isDead;


    protected override void Awake()
    {
        base.Awake();
        _waterObjectMass = GetComponent<NWH.DWP2.WaterObjects.MassFromVolume>();
        if (_waterObjectMass == null)
            Debug.LogWarning("缺少水脚本！！");
    }
    protected override void Start()
    {
        base.Start();
        _currentUptakeTime = 0;
        float randSize = Random.Range(_startSize.x, _startSize.y);
        transform.localScale = new(randSize, 0.5f, randSize);
        _meltingSize = randSize;
        _size = randSize;
        SetMassFromMaterial();
        _isDead = false;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //插值减少大小
        if (_size != _meltingSize)
            _size = Mathf.Lerp(_size, _meltingSize, 0.01f);
        //同步大小
        if (_size != transform.localScale.x)
        {
            transform.localScale = new Vector3(_size, transform.localScale.y, _size);
            SetMassFromMaterial();//同步质量
        }
    }
    void SetMassFromMaterial()
    {
        _waterObjectMass.CalculateAndApplyFromMaterial();
        _rb.mass = _waterObjectMass.mass;
    }

    protected override void OnCollisionStay(Collision other)
    {
        base.OnCollisionStay(other);
        if (!_isDead)
        {
            _currentUptakeTime -= Time.deltaTime;
            if (_currentUptakeTime < 0)
            {
                if (other.gameObject.CompareTag("Ice"))
                {
                    _meltingSize -= _minimizeValue;
                    //判断是否死亡
                    if (_meltingSize < _maxSmallSize)
                    {
                        _isDead = true;
                        _meltingSize = _maxSmallSize;
                    }
                }
                _currentUptakeTime = _uptakeTime;
            }
        }
    }
}
