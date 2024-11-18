using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite2dForLookAt : MonoBehaviour
{
    SpriteRenderer _sprite;

    public Transform _targetObj;
    public Rigidbody _targetRb;
    public Vector3 _offsetPos;

    public bool _isChangerAni = true;//是否根据方向切换动画

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _targetRb = _targetObj.GetComponent<Rigidbody>();
    }
    private void Start()
    {
        //和摄像机保持一样的倾斜
        transform.rotation = Camera.main.transform.rotation;
    }

    private void Update()
    {
        //坐标永远跟随目标
        transform.position = _targetObj.position + _offsetPos;
        //面向摄像机
        transform.LookAt(transform.position + Camera.main.transform.forward);
        if (_isChangerAni)
        {
            ChangerDirAni();
        }
    }

    //自动翻转
    void ChangerDirAni()
    {
        //向左
        if (_targetRb.velocity.x < 0)
        {
            _sprite.flipX = true;
        }
        //向右
        else
        {
            _sprite.flipX = false;
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
