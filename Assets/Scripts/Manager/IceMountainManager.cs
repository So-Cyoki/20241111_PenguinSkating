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
    public Crest.ShapeFFT _shapeFFT;//海浪脚本
    public float _addShapeWeight;
    public float _addShapeWind;
    public float _shapeLerpSpeed;
    float _shapeWeight_original;
    float _shapeWind_original;
    float _shapeWeight_target;
    float _shapeWind_target;

    int _diffcultyCount = 1;//当前难度水平

    private void Start()
    {
        _shapeWeight_original = _shapeFFT._weight;
        _shapeWind_original = _shapeFFT._windTurbulence;
        _shapeWeight_target = _shapeFFT._weight;
        _shapeWind_target = +_shapeFFT._windTurbulence;
    }
    private void LateUpdate()
    {
        //逐步变大波浪
        if (_shapeWeight_target != _shapeFFT._weight)
        {
            _shapeFFT._weight = Mathf.Lerp(_shapeFFT._weight, _shapeWeight_target, _shapeLerpSpeed);
        }
        if (_shapeWind_target != _shapeFFT._windTurbulence)
        {
            _shapeFFT._windTurbulence = Mathf.Lerp(_shapeFFT._windTurbulence, _shapeWind_target, _shapeLerpSpeed);
        }
    }
    void CreatNewMap(Vector3 startPos)
    {
        //按照事件的广播，随机一个新的地图块并按照传入的坐标拼接
        int randMapNum = Random.Range(0, _iceMountainMapList.Count);
        GameObject newMap = Instantiate(_iceMountainMapList[randMapNum], _EnvironmentTrans);
        newMap.transform.position = startPos;
        //储存新建的地图块，用以一会删除
        _waitDestoryMap.Add(newMap);

        DiffcultyAdd();//难度增加
    }
    public void ResetMap()
    {
        //删除所有地图块
        foreach (var item in _waitDestoryMap)
        {
            Destroy(item);
        }
        _waitDestoryMap.Clear();

        //重置难度
        _diffcultyCount = 1;
        _shapeFFT._weight = _shapeWeight_original;
        _shapeFFT._windTurbulence = _shapeWind_original;
        _shapeWeight_target = _shapeFFT._weight;
        _shapeWind_target = +_shapeFFT._windTurbulence;
    }
    void DiffcultyAdd()
    {
        switch (_diffcultyCount)
        {
            case 1:
                _shapeWeight_target += _addShapeWeight;
                break;
            case 2:
                _seaWaveObj._lerpSpeed += _addWaveSpeed;
                _shapeWeight_target += _addShapeWeight;
                break;
            case 3:
                _shapeWeight_target += _addShapeWeight * 0.5f;
                _shapeWind_target += _addShapeWind;
                break;
            case 4:
                _seaWaveObj._lerpSpeed += _addWaveSpeed;
                _shapeWeight_target += _addShapeWeight * 0.5f;
                _shapeWind_target += _addShapeWind * 0.5f;
                break;
            case 5:
                _seaWaveObj._lerpSpeed += _addWaveSpeed;
                _shapeWeight_target += _addShapeWeight * 0.5f;
                _shapeWind_target += _addShapeWind * 0.5f;
                break;
        }
        if (_shapeWeight_target > 1)
            _shapeWeight_target = 1;
        if (_shapeWind_target > 1)
            _shapeWind_target = 1;

        _diffcultyCount++;//难度+1
    }

    private void OnEnable()
    {
        IceMountainMap.OnMoveMiddle += CreatNewMap;
        IceMountainMap.OnMoveLast += DiffcultyAdd;
    }
    private void OnDisable()
    {
        IceMountainMap.OnMoveMiddle -= CreatNewMap;
        IceMountainMap.OnMoveLast -= DiffcultyAdd;
    }
}
