using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody _rb;
    NWH.DWP2.WaterObjects.WaterObject _waterObject;//水插件脚本

    float _submergedVolume;//浮力
    public float _speed;
    public float _rotationSpeed;
    public float _jumpSpeed;

    bool _isJump;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _waterObject = GetComponent<NWH.DWP2.WaterObjects.WaterObject>();
    }

    private void Update()
    {
        Move();
    }
    private void FixedUpdate()
    {
        //是否进入水中判断
        _submergedVolume = _waterObject.submergedVolume;
        if (_submergedVolume > 1f && _rb.velocity.y < 0)
            _isJump = false;
    }

    void Move()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 moveDir = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDir.magnitude > 0.1f)
        {
            //旋转
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            //移动
            _rb.velocity = new(moveDir.x * _speed, _rb.velocity.y, moveDir.z * _speed);
        }
        //跳跃
        if (Input.GetKeyDown(KeyCode.Space) && !_isJump)
        {
            _rb.AddForce(_rb.mass * _jumpSpeed * Vector3.up, ForceMode.Impulse);
            _isJump = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ice"))
        {
            _isJump = false;
        }
    }
}
