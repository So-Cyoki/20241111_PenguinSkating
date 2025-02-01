using System;
using UnityEngine;

public class IcePlane : MonoBehaviour
{
    Rigidbody _rb;
    AudioSource _audio;
    NWH.DWP2.WaterObjects.MassFromVolume _waterObjectMass;//用来自动计算出质量的脚本
    SpringJoint _spJoint;
    public ParticleSystem _meltParticle;

    [Header("自动融化")]
    [Tooltip("单次融化减少多少")] public float _meltingValue;
    [Tooltip("多久融化一次")] public float _meltingTime;
    float _currentMeltingTime;
    [Tooltip("单次融化Lerp值")] public float _meltingLerp;
    public float _maxSmallSize;

    [Header("吸收变大")]
    public float _addValue;//增加的值
    [Tooltip("多久进行一次吸收")] public float _uptakeTime;
    [Tooltip("单次增加Lerp值")] public float _uptakeLerp;
    public float _maxBigSize;
    float _currentUptakeTime;
    float _meltingSize;//融化后的大小
    float _size;//当前的大小(厚度不变)
    //弹簧铰链
    Item_icePlane _csItem;//捕获到的冰块脚本
    float _originalSpring;
    float _originalDamper;
    Vector3 _originalPos;
    Quaternion _originalRotation;
    Vector3 _originalScale;

    int _kidCount;

    bool _isDead;
    bool _isEndToWave;

    public static event Action<int> OnKidCount;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _audio = GetComponent<AudioSource>();
        _spJoint = GetComponent<SpringJoint>();
        _waterObjectMass = GetComponent<NWH.DWP2.WaterObjects.MassFromVolume>();
        if (_waterObjectMass == null)
            Debug.LogWarning("缺少水脚本！！");

        _originalPos = transform.position;
        _originalRotation = transform.rotation;
        _originalScale = transform.localScale;
    }
    private void Start()
    {
        _currentMeltingTime = 0;
        _currentUptakeTime = 0;
        _meltingSize = transform.localScale.x;
        _size = transform.localScale.x;
        SetMassFromMaterial();
        _originalSpring = _spJoint.spring;
        _originalDamper = _spJoint.damper;
        _spJoint.spring = 0;
        _spJoint.damper = 0;
    }
    private void Update()
    {
        //判断是否将别的冰块吸收殆尽，就不要再用铰链连着了
        if (_csItem != null && _csItem._isDead)
        {
            _csItem = null;
            _spJoint.connectedBody = null;
            _spJoint.spring = 0;
            _spJoint.damper = 0;
        }
    }
    private void FixedUpdate()
    {
        //触发自动融化(并且判断是否死亡)
        if (!_isDead)
        {
            _currentMeltingTime += Time.deltaTime;
            if (_currentMeltingTime > _meltingTime)
            {
                _meltingSize -= _meltingValue;
                if (_meltingSize < _maxSmallSize)
                {
                    _isDead = true;
                    _meltingSize = _maxSmallSize;
                }
                _currentMeltingTime = 0;
                //缩小的粒子特效
                if (_meltParticle.transform.localScale.x > 0.5f)
                    _meltParticle.transform.localScale = new Vector3(_meltingSize, _meltingSize, _meltingSize);
                else
                    _meltParticle.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                _meltParticle.Play();
            }
        }
        //吸收增加大小
        if (_csItem != null)
        {
            _currentUptakeTime += Time.deltaTime;
            if (_currentUptakeTime > _uptakeTime)
            {
                _meltingSize += _addValue;
                if (_meltingSize > _maxBigSize)
                    _meltingSize = _maxBigSize;
                _currentUptakeTime = 0;
            }
        }
        //线性插值改变大小
        if (_size >= _meltingSize)
            _size = Mathf.Lerp(_size, _meltingSize, _meltingLerp);
        if (_size <= _meltingSize)
            _size = Mathf.Lerp(_size, _meltingSize, _uptakeLerp);
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
    public void Inital()
    {
        transform.SetPositionAndRotation(_originalPos, _originalRotation);
        transform.localScale = _originalScale;
        _meltingSize = transform.localScale.x;
        _size = transform.localScale.x;
        SetMassFromMaterial();
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _currentMeltingTime = 0;
        _currentUptakeTime = 0;
        _kidCount = 0;
        _isEndToWave = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        //当冰块碰到海浪，就代表游戏结束了
        if (other.gameObject.CompareTag("SeaWave"))
        {
            _isEndToWave = true;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Item_kid") && !_isEndToWave)
        {
            _kidCount++;//记录Kid数量
            OnKidCount?.Invoke(_kidCount);
        }
        if (other.gameObject.CompareTag("Item_icePlane"))
        {
            if (_csItem == null)
                _csItem = other.gameObject.GetComponent<Item_icePlane>();
            _spJoint.connectedBody = other.rigidbody;
            _spJoint.spring = _originalSpring;
            _spJoint.damper = _originalDamper;
        }
        //播放声音
        if (other.gameObject.CompareTag("Item_fish")
        || other.gameObject.CompareTag("Item_kid")
        || other.gameObject.CompareTag("Item_icePlane")
        || other.gameObject.CompareTag("Item_smallIce")
        || other.gameObject.CompareTag("IceMountain")
        || other.gameObject.CompareTag("Player"))
        {
            _audio.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            //_audio.Play();
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Item_kid") && !_isEndToWave)
        {
            _kidCount--;//记录Kid数量
            OnKidCount?.Invoke(_kidCount);
        }
        //如果想要更黏住的吸收效果，就不要下面的这段代码
        // if (other.gameObject.CompareTag("Item_icePlane"))
        // {
        //     _csItem = null;
        //     _spJoint.connectedBody = null;
        //     _spJoint.spring = 0;
        //     _spJoint.damper = 0;
        // }
    }
}
