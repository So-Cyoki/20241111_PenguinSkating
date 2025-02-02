using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    Rigidbody _rb;
    NWH.DWP2.WaterObjects.WaterObject _waterObject;//水插件脚本

    [Tooltip("P1:1,P2:2")] public int _playerIndex = 1;
    [HideInInspector] public PlayerInput _playerInput;
    [HideInInspector] public CatchCollision _catchCollisionCS;
    public Transform _startPos;
    public ParticleSystem _particleWater;
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
    public float _catchRumble = 0.2f;
    public float _catchRumbleTime = 0.1f;
    public float _jumpRumble = 0.2f;
    public float _jumpRumbleTime = 0.1f;
    public float _runRumble = 0.2f;
    public float _runRumbleTime = 0.01f;
    [HideInInspector] public Coroutine _coroutineRumble;
    [Header("音乐")]
    public AudioSource _audio;
    public AudioSource _audio2;
    public AudioSource _audio3;
    public AudioClip _clipCatch;
    public AudioClip _clipDrop;
    public AudioClip _clipDropWater;
    public AudioClip _clipJump;
    public AudioClip _clipRush;

    Vector3 _originalPos;
    Quaternion _originalRotation;

    bool _isJump;
    bool _isWater;
    bool _isRun;

    public static event Action<int> OnScoreUpdate;
    public static event Action<int, float, float, bool> OnStaminaUpdate;

    private void Awake()
    {
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
        Move();
    }
    private void LateUpdate()
    {
        //是否进入水中判断
        _submergedVolume = _waterObject.submergedVolume;
        if (_submergedVolume > 1f && _rb.velocity != Vector3.zero)
        {
            _isWater = true;
            _arrowMark.position = new(_arrowMark.position.x + _arrowWaterPosOffset.x, _arrowWaterPosOffset.y, _arrowMark.position.z + _arrowWaterPosOffset.z);
            PlayAudio(_clipDropWater, 2);
            if (_rb.velocity.y < 0)
                _isJump = false;//跳跃
        }
        else
        {
            _arrowMark.localPosition = _arrowStandPosOffset;
        }
        //传输距离为分数
        int length = (int)(transform.position - _startPos.position).magnitude;
        if (_highScore < length)
        {
            _highScore = length;
            OnScoreUpdate?.Invoke(_highScore);
        }
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
        //获取当前的控制输入 
        InputActionMap currentActionMap = _playerInput.actions.FindActionMap(_playerInput.currentActionMap.name);//获取当前的ActionMap(因为是双人游戏，所以必须先获取当前的ActionMap)
        InputAction moveAction = currentActionMap.FindAction("Move");//根据当前的ActionMap来分配Action
        InputAction runAction = currentActionMap.FindAction("Run");
        InputAction jumpAction = currentActionMap.FindAction("Jump");
        //最后把Action转成最终的按键
        Vector2 dir = moveAction.ReadValue<Vector2>();
        bool isRunPressed = runAction.IsPressed();
        bool isJumpPressed = jumpAction.WasPressedThisFrame();

        Vector3 moveDir = new Vector3(dir.x, 0, dir.y).normalized;

        float currentSpeed = _speed;
        bool isUseStamina = false;
        //冲刺
        if (isRunPressed)
        {
            if (_currentStamina > 0 && !_isUseStaminaOver)
            {
                currentSpeed = _runSpeed;
                _currentStamina -= _runStamina;
                OnStaminaUpdate?.Invoke(_playerIndex, _maxStamina, _currentStamina, _isUseStaminaOver);
                isUseStamina = true;
                if (_currentStamina <= 0)
                    _isUseStaminaOver = true;
                if (!_particleWater.isPlaying && _isWater)
                    _particleWater.Play();//粒子效果(不知道为什么不循环就会出现粒子效果不显示的问题)
                GamepadRumble(_runRumble, _runRumble, _runRumbleTime);//手柄震动
                if (!_isRun)
                    PlayAudio(_clipRush, 3);
                _isRun = true;
            }
            else
            {
                _isRun = false;
                if (_particleWater.isPlaying)
                    _particleWater.Stop();
            }
        }
        else
        {
            _isRun = false;
            if (_particleWater.isPlaying)
                _particleWater.Stop();
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
        if (isJumpPressed && !_isJump)
        {
            _isJump = true;
            _rb.AddForce(_jumpForce * Vector3.up, ForceMode.VelocityChange);
            PlayAudio(_clipJump, 1);
            GamepadRumble(_jumpRumble, _jumpRumble, _jumpRumbleTime);//手柄震动
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
    public void GamepadRumble(float low, float high, float time)
    {
        if (_coroutineRumble != null)
            StopCoroutine(_coroutineRumble);
        _coroutineRumble = StartCoroutine(EnumGamepadRumble(low, high, time));
    }
    IEnumerator EnumGamepadRumble(float low, float high, float time)
    {
        if (Gamepad.current == null)
            yield break;
        Gamepad.current.SetMotorSpeeds(low, high);// 设置震动频率
        yield return new WaitForSecondsRealtime(time);
        Gamepad.current.SetMotorSpeeds(0, 0); // 停止震动
    }
    //音乐播放
    public void PlayAudio(AudioClip clip, int index)
    {
        switch (index)
        {
            case 1:
                _audio.Stop();
                _audio.clip = clip;
                _audio.Play();
                break;
            case 2:
                _audio2.Stop();
                _audio2.clip = clip;
                _audio2.Play();
                break;
            case 3:
                _audio3.Stop();
                _audio3.clip = clip;
                _audio3.Play();
                break;
        }
    }

    private void OnDisable()
    {
        Gamepad.current?.SetMotorSpeeds(0, 0); // 停止震动
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ice")
            || other.gameObject.CompareTag("Item_icePlane")
            || other.gameObject.CompareTag("IceMountain"))
        {
            _isWater = false;
            if (_rb.velocity.y < 0)
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
