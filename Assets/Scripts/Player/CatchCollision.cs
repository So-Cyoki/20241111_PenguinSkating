using UnityEngine;
using UnityEngine.InputSystem;

public class CatchCollision : MonoBehaviour
{
    public Transform _itemParent;
    Player _playerCS;
    public ParticleSystem _particleDrop;
    public ParticleSystem _particleCatch;

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

        _playerCS = transform.parent.GetComponent<Player>();
    }
    private void Update()
    {
        //获取输入
        InputActionMap currentActionMap = _playerCS._playerInput.actions.FindActionMap(_playerCS._playerInput.currentActionMap.name);
        InputAction catchAction = currentActionMap.FindAction("Catch");
        InputAction dropAction = currentActionMap.FindAction("Drop");
        bool isCatchPressed = catchAction.WasPressedThisFrame();
        bool isDropPressed = dropAction.WasPressedThisFrame();

        if (isCatchPressed && _lastCatchObj != null && !_isCatch)
        {
            CatchThing();
            _playerCS.GamepadRumble(_playerCS._catchRumble, _playerCS._catchRumble, _playerCS._catchRumbleTime);
            _playerCS.PlayAudio(_playerCS._clipCatch, 1);
            _particleCatch.Play();
        }
        if (isDropPressed && _isCatch)
        {
            DropThing();
            _playerCS.GamepadRumble(_playerCS._catchRumble, _playerCS._catchRumble, _playerCS._catchRumbleTime);
            _playerCS.PlayAudio(_playerCS._clipDrop, 1);
            _particleDrop.Play();
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
        if (_catchTrans != null && _isCatch
        && (_catchTrans.localPosition - _catchPosOffset).sqrMagnitude > 0.001f)
        {
            _catchTrans.localPosition = Vector3.MoveTowards(_catchTrans.localPosition, _catchPosOffset, _catchSpeed);
            _catchRb.velocity = Vector3.zero;
            _catchRb.angularVelocity = Vector3.zero;
            //_catchTrans.localPosition = Vector3.Lerp(_catchTrans.localPosition, _catchPosOffset, _catchSpeed * Time.fixedDeltaTime);
            //_catchRb.MovePosition(Vector3.MoveTowards(_catchTrans.position, transform.position, _catchSpeed));
        }
    }

    void CatchThing()
    {
        //Debug.Log("抓起来");
        //首先判断一下是否被Player拿着,是的情况就先把拿着物体的那个Player的CatchThing置空
        if (IsCatchThing(_lastCatchObj))
        {
            Player playerCS = _lastCatchObj.transform.parent.GetComponent<Player>();
            playerCS._catchCollisionCS.CatchThingRob();
        }
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
    void DropThing()
    {
        //Debug.Log("放下");
        _catchRb.useGravity = true;
        _catchRb.velocity = Vector3.zero;
        _catchRb.angularVelocity = Vector3.zero;

        //Vector3 dir = (Quaternion.Euler(_throwAngle) * transform.forward).normalized;
        Vector3 dir = (Quaternion.LookRotation(transform.forward) * Quaternion.Euler(_throwAngle) * Vector3.forward).normalized;
        _catchRb.AddForce(_throwForce * dir, ForceMode.VelocityChange);
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
    //拿着的东西被抢走
    void CatchThingRob()
    {
        _catchTrans = null;
        _catchObj = null;
        _catchRb = null;
        _catchSprite = null;
        _lastCatchObj = null;
        _isCatch = false;
    }
    //判断当前物体是否被Player拿着
    bool IsCatchThing(GameObject obj)
    {
        if (obj == null)
            return false;
        Transform parent = obj.transform.parent;
        if (parent.CompareTag("Player"))
            return true;
        else
            return false;
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
        Item_kid.OnEatFood += CatchThingEat;
    }
    private void OnDisable()
    {
        Item_kid.OnEatFood += CatchThingEat;
    }
}
