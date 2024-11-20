using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchCollision : MonoBehaviour
{
    Transform _catchTrans;
    Rigidbody _catchRb;
    ItemBase _catchSprite;
    public GameObject _lastCatchObj;//最后进入抓取范围的物体
    public Transform _itemParent;

    public Vector3 _transOffset;
    public float _throwForce;
    public Vector3 _throwDir;
    Vector3 _catchScale;//被抓起物体的原本的大小

    public bool _isCatch;

    private void Update()
    {
        //抓起来
        if (Input.GetKeyDown(KeyCode.J) && _lastCatchObj != null)
        {
            if (!_isCatch)
            {
                Debug.Log("抓起来");
                _catchTrans = _lastCatchObj.transform;
                _catchScale = _catchTrans.localScale;
                _catchRb = _catchTrans.GetComponent<Rigidbody>();
                _catchRb.velocity = Vector3.zero;
                _catchRb.useGravity = false;
                _catchSprite = _catchTrans.GetComponent<ItemBase>();
                _catchSprite._itemState = ItemState.CATCH;

                _catchTrans.SetParent(transform);

                _isCatch = true;
            }
        }
        //放下去
        if (Input.GetKeyDown(KeyCode.K) && _isCatch)
        {
            Debug.Log("放下");
            _catchRb.useGravity = true;
            Vector3 dir = (transform.right * _throwDir.x + transform.up * _throwDir.y + transform.forward * _throwDir.z).normalized;
            _catchRb.AddForce(_throwForce * _catchRb.mass * dir, ForceMode.Impulse);
            _catchTrans.SetParent(_itemParent);
            _catchTrans.localScale = _catchScale;
            _catchSprite._itemState = ItemState.WATER;

            _isCatch = false;
        }
    }
    private void LateUpdate()
    {
        //改变抓取物体坐标
        if (_isCatch)
        {
            _catchTrans.localPosition = _transOffset;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //记录最后一个进入判定范围的
        if (other.gameObject.CompareTag("Item"))
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
}
