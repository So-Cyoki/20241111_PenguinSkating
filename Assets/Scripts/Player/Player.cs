using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody _rb;
    NWH.DWP2.WaterObjects.WaterObject _waterObject;//水插件脚本

    PlayerInputActions _inputActions;
    public Transform _startPos;
    float _submergedVolume;//浮力
    public float _speed;
    public float _jumpForce;
    public float _runSpeed;
    [Header("体力变量")]
    public float _maxStamina;
    float _currentStamina;
    public float _addStamina;
    public float _runStamina;
    int _highScore;
    [Header("指示箭头")]
    public Transform _arrowMark;
    public Vector3 _arrowStandPosOffset;
    public Vector3 _arrowWaterPosOffset;

    Vector3 _originalPos;
    Quaternion _originalRotation;

    bool _isJump;

    public static event Action<int> OnScoreUpdate;
    public static event Action<float, float> OnStaminaUpdate;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _rb = GetComponent<Rigidbody>();
        _waterObject = GetComponent<NWH.DWP2.WaterObjects.WaterObject>();
        _originalPos = transform.position;
        _originalRotation = transform.rotation;
    }
    private void Start()
    {
        _currentStamina = _maxStamina;
    }
    private void Update()
    {
        Move();
    }
    private void FixedUpdate()
    {
        _submergedVolume = _waterObject.submergedVolume;
        //是否进入水中判断
        if (_submergedVolume > 1f && _rb.velocity != Vector3.zero)
        {
            _arrowMark.position = new(_arrowMark.position.x + _arrowWaterPosOffset.x, _arrowWaterPosOffset.y, _arrowMark.position.z + _arrowWaterPosOffset.z);
            if (_rb.velocity.y < 0)
                _isJump = false;//跳跃
        }
        else
        {
            _arrowMark.localPosition = _arrowStandPosOffset;
        }
    }
    private void LateUpdate()
    {
        //传输距离为分数
        int length = (int)(transform.position - _startPos.position).magnitude;
        if (_highScore < length)
        {
            _highScore = length;
            OnScoreUpdate?.Invoke(_highScore);
        }
    }
    private void OnEnable()
    {
        _inputActions.Enable();
    }
    private void OnDisable()
    {
        _inputActions.Disable();
    }

    public void Inital()
    {
        transform.SetPositionAndRotation(_originalPos, _originalRotation);
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _highScore = 0;
    }

    void Move()
    {
        Vector2 dir = _inputActions.GamePlay.Move.ReadValue<Vector2>();
        // float vertical = Input.GetAxis("Vertical");
        // float horizontal = Input.GetAxis("Horizontal");
        // Vector3 moveDir = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 moveDir = new Vector3(dir.x, 0, dir.y).normalized;

        float currentSpeed = _speed;
        bool isUseStamina = false;
        //冲刺
        if (_inputActions.GamePlay.Run.IsPressed())
        {
            if (_currentStamina > 0)
            {
                currentSpeed = _runSpeed;
                _currentStamina -= _runStamina;
                OnStaminaUpdate?.Invoke(_maxStamina, _currentStamina);
                isUseStamina = true;
            }
        }
        if (moveDir.magnitude > 0.1f)
        {
            //旋转
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = targetRotation;
            //移动
            _rb.velocity = new(moveDir.x * currentSpeed, _rb.velocity.y, moveDir.z * currentSpeed);
        }
        //跳跃
        if (_inputActions.GamePlay.Jump.WasPressedThisFrame() && !_isJump)
        {
            _rb.AddForce(_rb.mass * _jumpForce * Vector3.up, ForceMode.Impulse);
            _isJump = true;
        }
        //恢复体力
        if (!isUseStamina)
        {
            if (_currentStamina < _maxStamina)
            {
                _currentStamina += _addStamina;
                OnStaminaUpdate?.Invoke(_maxStamina, _currentStamina);
            }
            else
            {
                _currentStamina = _maxStamina;
                OnStaminaUpdate?.Invoke(_maxStamina, _currentStamina);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ice") && _rb.velocity.y < 0)
        {
            _isJump = false;
        }
        //如果在水中碰到冰块，就是想要推冰块的时候，稍微调整一下Player的Y坐标
        //但是实现出来的效果不是特别好，先不使用吧
        // if (other.gameObject.CompareTag("Ice") && _submergedVolume > 0.1f)
        // {
        //     Vector3 pushPos = new(transform.position.x, 0.3f, transform.position.z);
        //     Vector3 rbPos = Vector3.MoveTowards(_rb.position, pushPos, _speed * Time.fixedDeltaTime);
        //     _rb.MovePosition(rbPos);
        // }
    }
}
