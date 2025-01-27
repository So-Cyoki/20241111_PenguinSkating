using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody _rb;
    NWH.DWP2.WaterObjects.WaterObject _waterObject;//水插件脚本

    [Tooltip("P1:1,P2:2")] public int _playerIndex = 1;
    PlayerInputActions _inputActions;
    PlayerInput _playerInput;
    [HideInInspector] public CatchCollision _catchCollisionCS;
    public Transform _startPos;
    public ParticleSystem _particle;
    float _submergedVolume;//浮力
    public float _speed;
    public float _jumpForce;
    public float _runSpeed;
    [Header("体力变量")]
    public float _maxStamina;
    float _currentStamina;
    public float _addStamina;
    public float _runStamina;
    [Tooltip("恢复多少体力后才可以再次奔跑")] public float _staminaRecoverRun = 0.5f;
    public bool _isUseStaminaOver;
    int _highScore;
    [Header("指示箭头")]
    public Transform _arrowMark;
    public Vector3 _arrowStandPosOffset;
    public Vector3 _arrowWaterPosOffset;
    [Header("手柄震动")]
    public float _lowFrequency = 0.2f;
    public float _highFrequency = 0.2f;
    public float _time_Rumble = 0.1f;

    Vector3 _originalPos;
    Quaternion _originalRotation;

    bool _isJump;

    public static event Action<int> OnScoreUpdate;
    public static event Action<int, float, float, bool> OnStaminaUpdate;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody>();
        _waterObject = GetComponent<NWH.DWP2.WaterObjects.WaterObject>();
        _originalPos = transform.position;
        _originalRotation = transform.rotation;
    }
    private void Start()
    {
        _catchCollisionCS = transform.Find("CatchCollison").GetComponent<CatchCollision>();
        if (_catchCollisionCS == null)
            Debug.LogError("CatchCollision is null");
        _currentStamina = _maxStamina;
    }
    private void Update()
    {
        //其实应该用PlayerInput的组件的方式，但是我懒得弄了，2P就直接这样用Move2
        switch (_playerIndex)
        {
            case 1:
                Move();
                break;
            case 2:
                Move_2P();
                break;
        }
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
        Vector3 moveDir = new Vector3(dir.x, 0, dir.y).normalized;

        float currentSpeed = _speed;
        bool isUseStamina = false;
        if (_inputActions.GamePlay.Run.IsPressed())
        {
            Debug.Log("Run");
        }
        //冲刺
        if (_playerInput.actions.FindAction("Run").triggered)
        {
            if (_currentStamina > 0 && !_isUseStaminaOver)
            {
                currentSpeed = _runSpeed;
                _currentStamina -= _runStamina;
                OnStaminaUpdate?.Invoke(_playerIndex, _maxStamina, _currentStamina, _isUseStaminaOver);
                isUseStamina = true;
                if (_currentStamina <= 0)
                    _isUseStaminaOver = true;
                _particle.Play();//粒子效果
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
                OnStaminaUpdate?.Invoke(_playerIndex, _maxStamina, _currentStamina, _isUseStaminaOver);
            }
            else
            {
                _currentStamina = _maxStamina;
                OnStaminaUpdate?.Invoke(_playerIndex, _maxStamina, _currentStamina, _isUseStaminaOver);
            }
            if (_isUseStaminaOver && _currentStamina >= _maxStamina * _staminaRecoverRun)
                _isUseStaminaOver = false;
        }
    }
    void Move_2P()
    {
        Vector2 dir = _inputActions.GamePlay2.Move.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(dir.x, 0, dir.y).normalized;

        float currentSpeed = _speed;
        bool isUseStamina = false;
        //冲刺
        if (_inputActions.GamePlay2.Run.IsPressed())
        {
            if (_currentStamina > 0 && !_isUseStaminaOver)
            {
                currentSpeed = _runSpeed;
                _currentStamina -= _runStamina;
                OnStaminaUpdate?.Invoke(_playerIndex, _maxStamina, _currentStamina, _isUseStaminaOver);
                isUseStamina = true;
                if (_currentStamina <= 0)
                    _isUseStaminaOver = true;
                _particle.Play();//粒子效果
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
        if (_inputActions.GamePlay2.Jump.WasPressedThisFrame() && !_isJump)
        {
            _rb.AddForce(_rb.mass * _jumpForce * Vector3.up, ForceMode.Impulse);
            _isJump = true;
            StartCoroutine(GamepadRumble(_lowFrequency, _highFrequency, _time_Rumble));
        }
        //恢复体力
        if (!isUseStamina)
        {
            if (_currentStamina < _maxStamina)
            {
                _currentStamina += _addStamina;
                OnStaminaUpdate?.Invoke(_playerIndex, _maxStamina, _currentStamina, _isUseStaminaOver);
            }
            else
            {
                _currentStamina = _maxStamina;
                OnStaminaUpdate?.Invoke(_playerIndex, _maxStamina, _currentStamina, _isUseStaminaOver);
            }
            if (_isUseStaminaOver && _currentStamina >= _maxStamina * _staminaRecoverRun)
                _isUseStaminaOver = false;
        }
    }

    //手柄震动
    System.Collections.IEnumerator GamepadRumble(float low, float high, float time)
    {
        if (Gamepad.current == null)
            yield break;
        Gamepad.current.SetMotorSpeeds(low, high);// 设置震动频率
        yield return new WaitForSecondsRealtime(time);
        Gamepad.current.SetMotorSpeeds(0, 0); // 停止震动
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
