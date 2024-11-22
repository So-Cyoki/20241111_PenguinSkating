using System;
using UnityEngine;

public class Item_kid : ItemBase
{
    public SpriteRenderer _spriteRen;

    public Vector2 _speedForce;
    [Tooltip("输入角度")] public Vector3 _angleRun;//随机转多少角度移动
    public Vector2 _idleTime;
    public float _sitDownTime;
    public Vector2 _hungerTime;
    float _currentHungerTime;
    float _randHungerTime;//每次都会随机一个饥饿时间，吃饭后重置
    ItemState _preState;

    [HideInInspector] public bool _isHunger;

    public static event Action OnEatFood;


    protected override void Start()
    {
        _isHunger = false;
        _randHungerTime = UnityEngine.Random.Range(_hungerTime.x, _hungerTime.y);
    }

    protected override void Update()
    {
        base.Update();

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

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
        //肚子饿把鱼吃掉
        if (GetState() == ItemState.ICE
        && _isHunger
        && other.gameObject.CompareTag("Item_fish"))
        {
            OnEatFood?.Invoke();
            Destroy(other.gameObject);

            _currentHungerTime = 0;
            _randHungerTime = UnityEngine.Random.Range(_hungerTime.x, _hungerTime.y);
            _animator.SetTrigger("tIdle");
            _isHunger = false;
        }
    }
}
