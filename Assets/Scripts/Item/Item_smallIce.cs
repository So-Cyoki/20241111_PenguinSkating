using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_smallIce : ItemBase
{
    public Vector2 _sizeRange = new(3f, 3f);
    float _size;
    public float _bigStartTime = 1f;//变大到随机的大小所需时间
    float _currentBigStartTime;
    public Vector2 _angleSpeedRange;
    float _angleSpeed;
    public float _touchForce;

    protected override void Start()
    {
        base.Start();
        transform.localScale = new(0.5f, 0.5f, 0.5f);
        _size = Random.Range(_sizeRange.x, _sizeRange.y);
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        _angleSpeed = Random.Range(_angleSpeedRange.x, _angleSpeedRange.y);
    }
    protected override void Update()
    {
        base.Update();
        if (_currentBigStartTime < _bigStartTime)
        {
            StartBig();
        }
        else
        {
            transform.Rotate(0, _angleSpeed * Time.deltaTime, 0);
        }
    }

    void StartBig()
    {
        _currentBigStartTime += Time.deltaTime;
        float t = _currentBigStartTime / _bigStartTime;
        transform.localScale = Vector3.Lerp(new(0.5f, 0.5f, 0.5f), new(_size, _size, _size), t);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
        if (!other.collider.CompareTag("Water") || !other.collider.CompareTag("IceMountain")
        )
        {
            _rb.AddForce((transform.position - other.transform.position).normalized * _touchForce, ForceMode.VelocityChange);
            if (other.rigidbody != null)
            {
                other.rigidbody.AddForce((other.transform.position - transform.position).normalized * _touchForce, ForceMode.VelocityChange);
            }
        }
    }
}
