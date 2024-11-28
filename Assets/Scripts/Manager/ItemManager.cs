using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemWithProbability
{
    public GameObject item;
    public int probability;
}

public class ItemManager : MonoBehaviour
{
    public Transform _playerTrans;
    public Transform _itemParent;
    public GameObject _itemIcePlane;

    [Tooltip("生成的范围,以Player为中心")]
    public Vector2 _creatLength;
    [Tooltip("多久生成一次")] public float _creatTime;
    float _currentCreatTime;
    readonly int _creatPosCheck = 100;//要搜索多少次位置
    [Tooltip("总共几率加起来不要超过100%")]
    public List<ItemWithProbability> _itemList;

    private void FixedUpdate()
    {
        _currentCreatTime += Time.deltaTime;
        if (_currentCreatTime > _creatTime)
        {
            RandCreatItem();
            _currentCreatTime = 0;
        }
    }

    void RandCreatItem()
    {
        int randNum = Random.Range(0, 101);
        int currentRandNum = 0;
        foreach (ItemWithProbability item in _itemList)
        {
            //按照概率生成物体
            currentRandNum += item.probability;
            if (randNum <= currentRandNum)
            {
                //随机坐标
                Vector3 creatPos = Vector3.zero;
                for (int i = 0; i < _creatPosCheck; i++)
                {
                    //随机生成方向和距离
                    Vector3 randDir = new Vector3(Random.Range(-0.2f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                    float randLength = Random.Range(_creatLength.x, _creatLength.y);
                    //发射射线(如果想要省性能，可以只检测冰山的物理层)
                    Ray ray = new(_playerTrans.position, randDir);
                    if (Physics.Raycast(ray, out RaycastHit hit, randLength))
                    {
                        if (hit.collider.CompareTag("IceMountain")
                        || hit.collider.CompareTag("Ice")
                        || hit.collider.CompareTag("Item"))
                        {
                            continue; //重新随机位置
                        }
                    }
                    //射线没有碰到不允许的物体，位置允许生成
                    creatPos = _playerTrans.position + randDir * randLength;
                    break;
                }
                //生成物体
                GameObject itemObj = Instantiate(item.item, _itemParent);
                ItemBase itemBase = itemObj.GetComponent<ItemBase>();
                itemBase.Initial(creatPos, _playerTrans);
                break;
            }
        }
    }
}
