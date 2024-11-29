using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaWaveSpriteAnimation : MonoBehaviour
{
    [Tooltip("振幅,控制波浪的高度")] public Vector2 randAmplitude;
    [Tooltip("频率,控制波浪的快慢")] public Vector2 randFrequency;
    float originalY;
    float amplitude;
    float frequency;

    private void Start()
    {
        originalY = transform.localPosition.y;
        amplitude = Random.Range(randAmplitude.x, randAmplitude.y);
        frequency = Random.Range(randFrequency.x, randFrequency.y);
    }
    private void FixedUpdate()
    {
        // 计算波浪的偏移
        float wave = Mathf.Sin(Time.fixedTime * frequency) * amplitude;
        // 更新物体的本地位置，仅修改Y轴
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            originalY + wave,
            transform.localPosition.z
        );
    }
}
