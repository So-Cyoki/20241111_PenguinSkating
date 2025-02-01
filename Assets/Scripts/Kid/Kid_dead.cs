using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid_dead : KidStatesBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        main._rb.AddForce(main._speedForce.y * new Vector3(0.3f, 1, 0), ForceMode.VelocityChange);
        main.PlayAudio(main._clipDead, false);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);
        //设计了一个延时关闭碰撞体，不然冰块那边会因为无法检测到碰撞体的离开而出现计数问题
        if (main._coll.enabled && stateInfo.normalizedTime > 0.01f)
        {
            main._coll.enabled = false;
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        Destroy(animator.gameObject);
    }
}
