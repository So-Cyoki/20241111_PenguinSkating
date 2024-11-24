using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid_Move : KidStatesBase
{
    int _rotationNums = 10;//最多寻找多少次角度
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var main = GetKidMain(animator);

        //随机找一个角度(不会掉下冰块的角度)
        for (int i = 0; i < _rotationNums; i++)
        {
            Quaternion randQuaternion = Quaternion.Euler(
                Random.Range(-main._angleRun.x, main._angleRun.x),
                Random.Range(-main._angleRun.y, main._angleRun.y),
                Random.Range(-main._angleRun.z, main._angleRun.z));
            Vector3 randDir = randQuaternion * Vector3.forward;
            animator.transform.rotation = Quaternion.LookRotation(randDir, Vector3.up);
            if (Physics.Raycast(main.transform.TransformPoint(main._originOffset), main.transform.TransformDirection(main._rayDir), main._rayLength))
            {
                break;
            }
            //Debug.Log("重新找方向");
        }
        //随机一个速度
        float randSpeedForce = Random.Range(main._speedForce.x, main._speedForce.y);
        //移动
        if (main.GetState() == ItemState.ICE)
        {
            main._rb.AddForce(randSpeedForce * main._rb.mass * animator.transform.forward, ForceMode.Impulse);
            //Debug.Log("小企鹅移动");
        }

        animator.SetTrigger("tIdle");
    }
}
