using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePlane : MonoBehaviour
{
    Rigidbody _rb;
    NWH.DWP2.WaterObjects.MassFromVolume _waterObjectMass;//用来自动计算出质量的脚本

    [Tooltip("单次融化减少多少")] public float _meltingValue;
    [Tooltip("多久融化一次")] public float _meltingTime;
    float _currentMeltingTime;
    [Tooltip("单次融化使用的时间")] public float _meltingUseTime;
    public float _addSpeed;//增加的速度
    public float _minimizeSpeed;//吸收的速度
    [Tooltip("多久进行一次吸收")] public float _uptakeTime;
    float _currentUptakeTime;
    float _meltingSize;//融化后的大小
    float _size;//当前的大小(厚度不变)

    [HideInInspector] public bool _isDead;

    [Header("主冰块请勾选")]
    public bool _isMain;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _waterObjectMass = GetComponent<NWH.DWP2.WaterObjects.MassFromVolume>();
        if (_waterObjectMass == null)
            Debug.LogWarning("缺少水脚本！！");
    }
    private void Start()
    {
        _currentMeltingTime = 0;
        _currentUptakeTime = 0;
        _meltingSize = transform.localScale.x;
        _size = transform.localScale.x;
        SetMassFromMaterial();
        _isDead = false;
    }
    private void Update()
    {
        if (_isMain)
        {
            //触发融化
            _currentMeltingTime += Time.deltaTime;
            if (_currentMeltingTime > _meltingTime)
            {
                _meltingSize -= _meltingValue;
                _currentMeltingTime = 0;
            }
        }
        if (_size != _meltingSize)
        {
            _size = Mathf.Lerp(_size, _meltingSize, Time.deltaTime / _meltingUseTime);
            SetMassFromMaterial();
        }
        //判断是否死亡
        if (_meltingSize < 0.1f)
        {
            _isDead = true;
            _meltingSize = 0.1f;
        }
    }
    private void FixedUpdate()
    {
        //同步大小
        if (_size != transform.localScale.x)
        {
            transform.localScale = new Vector3(_size, transform.localScale.y, _size);
        }
    }
    void SetMassFromMaterial()
    {
        _waterObjectMass.CalculateAndApplyFromMaterial();
        _rb.mass = _waterObjectMass.mass;
    }

    private void OnCollisionStay(Collision other)
    {
        _currentUptakeTime -= Time.deltaTime;
        if (_currentUptakeTime < 0)
        {
            if (_isMain)
            {
                //主冰块的情况
                if (other.gameObject.CompareTag("Ice")
                && !other.gameObject.GetComponent<IcePlane>()._isDead)
                {
                    _meltingSize += _addSpeed;
                }
            }
            else
            {
                //其余冰块的情况
                if (other.gameObject.CompareTag("Ice"))
                {
                    _meltingSize -= _minimizeSpeed;
                }
            }
            _currentUptakeTime = _uptakeTime;
        }
    }
}
