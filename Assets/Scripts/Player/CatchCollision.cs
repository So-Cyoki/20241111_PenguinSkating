using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchCollision : MonoBehaviour
{
    [SerializeField] Transform _catchTrans;
    [SerializeField] Rigidbody _catchRb;
    [SerializeField] ItemBase _catchSprite;
    [SerializeField] GameObject _lastCatchObj;//最后进入抓取范围的物体
    public Transform _itemParent;

    public Vector3 _catchPosOffset;
    public float _catchSpeed;
    public float _throwForce;
    public Vector3 _throwAngle;
    Vector3 _catchScale;//被抓起物体的原本的大小

    bool _isCatch;

    private void Update()
    {
        //抓起来
        if (Input.GetKeyDown(KeyCode.J) && _lastCatchObj != null)
        {
            if (!_isCatch)
            {
                //Debug.Log("抓起来");

                _catchTrans = _lastCatchObj.transform;
                _catchScale = _catchTrans.localScale;
                _catchRb = _catchTrans.GetComponent<Rigidbody>();
                _catchRb.velocity = Vector3.zero;
                _catchRb.useGravity = false;
                _catchSprite = _catchTrans.GetComponent<ItemBase>();
                _catchSprite.SetCatch(true);

                _catchTrans.SetParent(transform.parent);

                _isCatch = true;
            }
        }
        //放下去
        if (Input.GetKeyDown(KeyCode.K) && _isCatch)
        {
            //Debug.Log("放下");

            _catchRb.useGravity = true;
            _catchRb.velocity = Vector3.zero;
            Vector3 dir = (Quaternion.Euler(_throwAngle) * Vector3.forward).normalized;
            _catchRb.AddForce(_throwForce * _catchRb.mass * dir, ForceMode.Impulse);
            _catchTrans.SetParent(_itemParent);
            _catchTrans.localScale = _catchScale;

            _catchTrans = null;
            _catchRb = null;
            _catchSprite.SetCatch(false);
            _catchSprite = null;

            _isCatch = false;
        }
    }
    private void FixedUpdate()
    {
        //移动抓取目标到Offset位置
        if (_catchTrans != null && _catchTrans.localPosition != _catchPosOffset)
            _catchTrans.localPosition = Vector3.MoveTowards(_catchTrans.localPosition, _catchPosOffset, _catchSpeed);
    }

    //去掉拿着的东西
    void CatchThingNull()
    {
        _catchTrans = null;
        _catchRb = null;
        //_catchSprite.SetCatch(false);
        _catchSprite = null;
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
        Item_kid.OnEatFood += CatchThingNull;
    }
    private void OnDisable()
    {
        Item_kid.OnEatFood += CatchThingNull;
    }
}
