using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;      // 前后移动速度
    public float rotationSpeed = 100f; // 左右旋转速度
    private Rigidbody rb;

    void Start()
    {
        // 获取 Rigidbody 组件
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 获取输入
        float vertical = Input.GetAxis("Vertical");   // W/S 或 ↑/↓ 控制前后移动
        float horizontal = Input.GetAxis("Horizontal"); // A/D 或 ←/→ 控制旋转

        // 计算移动方向
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 如果有输入，就设置刚体速度
        if (moveDirection.magnitude > 0.1f)
        {
            rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
        }
    }
}
