using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite2dForLookAt : MonoBehaviour
{
    SpriteRenderer _sprite;

    public Transform _targetObj;
    Rigidbody _targetRb;
    public Vector3 _offsetPos;
    Vector3 _originalPos;

    [Tooltip("自动读取当前坐标来作为偏移值，手动偏移值将不再生效")]
    public bool _isAutoOffset = true;
    [Tooltip("是否根据方向切换动画")]
    public bool _isChangerAni = true;


    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _targetRb = _targetObj.GetComponent<Rigidbody>();
        _originalPos = transform.localPosition;
    }
    private void Start()
    {
        //和摄像机保持一样的倾斜
        transform.rotation = Camera.main.transform.rotation;
    }

    private void Update()
    {
        if (_isAutoOffset)
        {
            _offsetPos = _originalPos;
        }
        if (_isChangerAni)
        {
            ChangerDirAni();
        }
    }
    private void LateUpdate()
    {
        //面向摄像机
        transform.LookAt(transform.position + Camera.main.transform.forward);
        //坐标永远跟随目标
        transform.position = _targetObj.position + _offsetPos;
    }

    //自动翻转
    void ChangerDirAni()
    {
        //向左
        if (_targetRb.velocity.x < 1f)
        {
            _sprite.flipX = false;
        }
        //向右
        else
        {
            _sprite.flipX = true;
        }
        //向上
        if (_targetRb.velocity.y > 0)
        {

        }
        //向下
        else
        {

        }
    }
}
