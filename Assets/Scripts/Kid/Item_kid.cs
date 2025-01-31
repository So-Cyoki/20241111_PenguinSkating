using System;
using UnityEngine;

public class Item_kid : ItemBase
{
    [HideInInspector] public Sprite2dForLookAt _sprite2DForLookAtCS;
    [Header("基础属性")]
    public ParticleSystem _dropWaterParticle;
    public Vector2 _speedForce;
    [Tooltip("输入角度")] public Vector3 _angleRun;//随机转多少角度移动
    [Tooltip("边缘减速倍率")] public float _slowlyMultiply;
    public Vector2 _idleTime;
    public float _sitDownTime;

    [Header("饥饿属性")]
    public Vector2 _hungerTime;
    public float _hungerDeadTime;
    public float _hungerGodTime;//防止从别的状态计入饥饿状态，因为时间已经0的情况下立马死亡
    public Vector2 _hungerForce;
    public float _findFoodRadius;
    public float _findFoodTime;
    public bool _isDrawFindFoodRadius;
    Collider[] _findFoodColliders;

    float _currentHungerTime;
    float _randHungerTime;//每次都会随机一个饥饿时间，吃饭后重置
    [HideInInspector] public float _currentHungerDeadTime;
    ItemState _preState;

    [Header("射线属性")]
    public Vector3 _originOffset;
    public Vector3 _rayDir;
    public float _rayLength;
    public LayerMask _targetLayer;
    public bool _isDrawRay;

    [Header("音乐")]
    public AudioSource _audio;
    public AudioClip _clipDropWater;


    [HideInInspector] public bool _isHunger;

    public static event Action OnEatFood;

    protected override void Awake()
    {
        base.Awake();
        _findFoodColliders = new Collider[20];
    }
    protected override void Start()
    {
        base.Start();
        _sprite2DForLookAtCS = _spriteRen.GetComponent<Sprite2dForLookAt>();
        _isHunger = false;
        _randHungerTime = UnityEngine.Random.Range(_hungerTime.x, _hungerTime.y);
    }

    protected override void Update()
    {
        base.Update();

        //画一条射线
        if (_isDrawRay)
            Debug.DrawRay(transform.TransformPoint(_originOffset), transform.TransformDirection(_rayDir.normalized) * _rayLength, Color.red);

        //是否肚子饿判断
        if (!_isHunger)
        {
            _currentHungerTime += Time.deltaTime;
            if (_currentHungerTime >= _randHungerTime)
            {
                _isHunger = true;
            }
        }

        //是否掉下水判断
        if (_preState != GetState())
        {
            if (GetState() == ItemState.WATER)
            {
                _animator.SetTrigger("tDropWater");
            }
            _preState = GetState();
        }
    }

    //肚子饿的时候定时寻找食物
    void HungerFindFood()
    {
        int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, _findFoodRadius, _findFoodColliders);
        if (colliderCount <= 0)
            return;

        foreach (Collider collider in _findFoodColliders)
        {
            if (collider == null)
                continue;

            if (collider.CompareTag("Item_fish"))
            {
                if (collider.GetComponent<Item_fish>().GetState() == ItemState.ICE)
                {
                    Vector3 dir = collider.transform.position - transform.position;
                    _rb.AddForce(dir.normalized * UnityEngine.Random.Range(_hungerForce.x, _hungerForce.y), ForceMode.VelocityChange);
                    break;
                }
            }
        }
    }

    public void SetHungerFindFood(bool flag)
    {
        if (flag)
            InvokeRepeating(nameof(HungerFindFood), 0, _findFoodTime);
        else
            CancelInvoke(nameof(HungerFindFood));
    }
    //音乐播放
    public void PlayAudio(AudioClip clip)
    {
        _audio.clip = clip;
        _audio.Play();
    }

    private void OnDrawGizmos()
    {
        // 在场景视图中显示寻找食物的扫描范围
        if (_isDrawFindFoodRadius)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _findFoodRadius);
        }
    }
    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
        //肚子饿把鱼吃掉(如果想要难一些，可以加一个状态条件，必须在冰上的时候才吃东西)
        if (_isHunger && other.gameObject.CompareTag("Item_fish"))
        {
            Item_fish item_FishCS = other.gameObject.GetComponent<Item_fish>();
            item_FishCS.ToDead();
            OnEatFood?.Invoke();

            _currentHungerTime = 0;
            _currentHungerDeadTime = 0;
            _randHungerTime = UnityEngine.Random.Range(_hungerTime.x, _hungerTime.y);
            _animator.SetTrigger("tIdle");
            _isHunger = false;
        }
    }
}
