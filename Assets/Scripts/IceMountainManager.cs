using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:现在的对象还是直接新实例化还有直接删除，有性能影响的话就要改对象池了
public class IceMountainManager : MonoBehaviour
{
    public List<GameObject> _iceMountainMapList;//存储地图块
    List<GameObject> _waitDestoryMap = new();//等待删除的地图块

    void CreatNewMap(Vector3 startPos, GameObject oldMap)
    {
        //先删除之前的地图块
        if (_waitDestoryMap.Count != 0)
        {
            foreach (GameObject var in _waitDestoryMap)
            {
                Destroy(var);
            }
            _waitDestoryMap.Clear();
        }
        //按照事件的广播，随机一个新的地图块并按照传入的坐标拼接
        int randMapNum = Random.Range(0, _iceMountainMapList.Count);
        GameObject newMap = Instantiate(_iceMountainMapList[randMapNum], transform.parent);
        newMap.transform.position = startPos;
        //储存当前广播的地图块，用以一会删除
        _waitDestoryMap.Add(oldMap);
    }

    private void OnEnable()
    {
        IceMountain.OnMoveMiddle += CreatNewMap;
    }
    private void OnDisable()
    {
        IceMountain.OnMoveMiddle -= CreatNewMap;
    }
}
