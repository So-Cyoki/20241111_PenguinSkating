using UnityEngine;
using UnityEngine.InputSystem;

public class CatchCollision : MonoBehaviour
{
    PlayerInputActions _inputActions;
    public Transform _itemParent;

    public Vector3 _catchPosOffset;
    public float _catchSpeed;
    public float _throwForce;
    public Vector3 _throwAngle;
    Vector3 _catchScale;//被抓起物体的原本的大小

    [Header("检查用变量")]
    [SerializeField] Transform _catchTrans;
    [SerializeField] GameObject _catchObj;
    [SerializeField] Rigidbody _catchRb;
    [SerializeField] ItemBase _catchSprite;
    [SerializeField] GameObject _lastCatchObj;//最后进入抓取范围的物体
    [SerializeField] bool _isCatch;

    private void Awake()
    {
        _inputActions = new();
    }
    private void Update()
    {
        //抓起来
        if (_inputActions.GamePlay.Catch.WasPressedThisFrame() && _lastCatchObj != null)
        {
            if (!_isCatch)
            {
                //Debug.Log("抓起来");
                _catchTrans = _lastCatchObj.transform;
                _catchObj = _lastCatchObj;
                _catchScale = _catchTrans.localScale;
                _catchRb = _catchTrans.GetComponent<Rigidbody>();
                _catchRb.velocity = Vector3.zero;
                _catchRb.angularVelocity = Vector3.zero;
                _catchRb.useGravity = false;
                _catchSprite = _catchTrans.GetComponent<ItemBase>();
                _catchSprite.SetCatch(true);

                _catchTrans.SetParent(transform.parent);

                _isCatch = true;
            }
        }
        //放下去
        if (_inputActions.GamePlay.Drop.WasPressedThisFrame() && _isCatch)
        {
            //Debug.Log("放下");
            _catchRb.useGravity = true;
            _catchRb.velocity = Vector3.zero;
            _catchRb.angularVelocity = Vector3.zero;
            Vector3 dir = (Quaternion.Euler(_throwAngle) * Vector3.forward).normalized;
            _catchRb.AddForce(_throwForce * _catchRb.mass * dir, ForceMode.Impulse);
            _catchTrans.SetParent(_itemParent);
            _catchTrans.localScale = _catchScale;

            _catchSprite.SetCatch(false);
            _catchTrans = null;
            _catchObj = null;
            _catchRb = null;
            _catchSprite = null;
            _lastCatchObj = null;
            _isCatch = false;
        }
        //如果物体已经消失，那么就重置为空手状态
        if (_isCatch && _catchObj == null)
        {
            CatchThingNull();
        }
    }
    private void FixedUpdate()
    {
        //移动抓取目标到Offset位置
        if (_catchTrans != null && _catchTrans.localPosition != _catchPosOffset)
            _catchTrans.localPosition = Vector3.MoveTowards(_catchTrans.localPosition, _catchPosOffset, _catchSpeed);
    }

    //删除掉拿着的东西
    public void CatchThingNull()
    {
        if (_catchObj != null)
            Destroy(_catchTrans.gameObject);
        _catchTrans = null;
        _catchObj = null;
        _catchRb = null;
        _catchSprite = null;
        _lastCatchObj = null;
        _isCatch = false;
    }
    //拿着的东西被吃掉了
    void CatchThingEat()
    {
        if (_catchTrans != null)
            _catchTrans.SetParent(_itemParent);
        _catchTrans = null;
        _catchObj = null;
        _catchRb = null;
        _catchSprite = null;
        _lastCatchObj = null;
        _isCatch = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //记录最后一个进入判定范围的
        if (other.gameObject.CompareTag("Item_kid") || other.gameObject.CompareTag("Item_fish"))
        {
            _lastCatchObj = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == _lastCatchObj)
        {
            _lastCatchObj = null;
        }
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        Item_kid.OnEatFood += CatchThingEat;
    }
    private void OnDisable()
    {
        _inputActions.Disable();
        Item_kid.OnEatFood += CatchThingEat;
    }
}
