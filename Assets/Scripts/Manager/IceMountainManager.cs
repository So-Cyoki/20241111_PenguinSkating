using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:现在的对象还是直接新实例化还有直接删除，有性能影响的话就要改对象池了
public class IceMountainManager : MonoBehaviour
{
    public Transform _EnvironmentTrans;
    [Tooltip("最开始的地图位置")] public Vector3 _startPos;
    List<GameObject> _waitDestoryMap = new();//等待删除的地图块
    public List<GameObject> _iceMountainMapList;//存储地图块
    [Header("难度增加")]
    public SeaWave _seaWaveObj;
    public float _addWaveSpeed;

    void CreatNewMap(Vector3 startPos)
    {
        //按照事件的广播，随机一个新的地图块并按照传入的坐标拼接
        int randMapNum = Random.Range(0, _iceMountainMapList.Count);
        GameObject newMap = Instantiate(_iceMountainMapList[randMapNum], _EnvironmentTrans);
        newMap.transform.position = startPos;
        //储存新建的地图块，用以一会删除
        _waitDestoryMap.Add(newMap);

        //增加难度，让海浪加速
        _seaWaveObj._lerpSpeed += _addWaveSpeed;
    }
    public void ResetMap()
    {
        //删除所有地图块
        foreach (var item in _waitDestoryMap)
        {
            Destroy(item);
        }
        _waitDestoryMap.Clear();
    }

    private void OnEnable()
    {
        IceMountainMap.OnMoveMiddle += CreatNewMap;
    }
    private void OnDisable()
    {
        IceMountainMap.OnMoveMiddle -= CreatNewMap;
    }
}
