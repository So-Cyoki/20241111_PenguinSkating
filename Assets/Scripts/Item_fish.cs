using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_fish : MonoBehaviour
{
    Rigidbody _rb;
    public float _speedForce;

    public bool _isIce;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ice"))
        {
            _isIce = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ice"))
        {
            _isIce = false;
        }
    }
}
