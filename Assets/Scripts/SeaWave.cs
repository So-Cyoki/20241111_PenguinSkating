using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaWave : MonoBehaviour
{
    public GameObject _spriteObj;
    public Transform _playerTrans;

    [Header("基础属性")]
    public float _lerpSpeed;
    public float _waveForce;

    [Header("画图属性")]
    [Tooltip("画图的横竖多少列")] public Vector2 _drawNums;
    public Vector2 _drawOffsetPos;
    [Tooltip("每一列放大多少")] public float _drawAddSize;
    public Vector2 _drawRandOffsetPos;

    private void Start()
    {
        DrawSprite();
    }
    private void FixedUpdate()
    {
        Vector3 targetPos = new(_playerTrans.position.x, 0, _playerTrans.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, _lerpSpeed);
    }
    void DrawSprite()
    {
        Transform drawParent = _spriteObj.transform.parent;
        for (int i = 0; i < _drawNums.x; i++)
        {
            float intervalsX = _drawOffsetPos.y / 2 * (i % 2);//前后排的X轴是交错排列的
            for (int j = 1; j <= _drawNums.y; j++)
            {
                GameObject obj1 = Instantiate(_spriteObj, drawParent);
                GameObject obj2 = Instantiate(_spriteObj, drawParent);
                Vector3 pos1 = new(_drawOffsetPos.y * j + intervalsX, 0, _drawOffsetPos.x * i - (_drawAddSize * i * 10));
                Vector3 pos2 = new(-_drawOffsetPos.y * j - intervalsX, 0, _drawOffsetPos.x * i - (_drawAddSize * i * 10));
                pos1 += new Vector3(Random.Range(-_drawRandOffsetPos.y, _drawRandOffsetPos.y), 0, Random.Range(-_drawRandOffsetPos.x, _drawRandOffsetPos.x));
                pos2 += new Vector3(Random.Range(-_drawRandOffsetPos.y, _drawRandOffsetPos.y), 0, Random.Range(-_drawRandOffsetPos.x, _drawRandOffsetPos.x));
                obj1.transform.localPosition = pos1;
                obj2.transform.localPosition = pos2;
                float addSize = _drawAddSize * i;
                if (addSize == 0)
                    addSize = 1;
                obj1.transform.localScale *= addSize;
                obj2.transform.localScale *= addSize;
            }
            if (i < _drawNums.x - 1)
            {
                GameObject rowObj = Instantiate(_spriteObj, drawParent);
                Vector3 rowPos = new(intervalsX, 0, _drawOffsetPos.x * (i + 1) - (_drawAddSize * (i + 1) * 10));
                rowPos += new Vector3(Random.Range(-_drawRandOffsetPos.y, _drawRandOffsetPos.y), 0, Random.Range(-_drawRandOffsetPos.x, _drawRandOffsetPos.x));
                rowObj.transform.localPosition = rowPos;
                rowObj.transform.localScale *= _drawAddSize * (i + 1);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player")
        || other.gameObject.CompareTag("Ice")
        || other.gameObject.CompareTag("Item_fish")
        || other.gameObject.CompareTag("Item_kid")
        || other.gameObject.CompareTag("Item_icePlane"))
        {
            Vector3 dir = new(-1, 1, 0);
            other.attachedRigidbody.AddForce(dir * _waveForce, ForceMode.Acceleration);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")
        || other.gameObject.CompareTag("Ice"))
        {
            Debug.Log("Player被浪冲走了!游戏失败!");
        }
    }
}
