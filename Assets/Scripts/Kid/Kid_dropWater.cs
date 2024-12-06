using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid_dropWater : KidStatesBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        //main._spriteRen.color = Color.red;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        //动画Sprite翻转(因为如果在动画片段中加入Flip的翻转，对象的Flip就会被控制，代码中再也无法更改)
        if (stateInfo.normalizedTime % 1.5f > 0.75f)
        {
            main._spriteRen.flipX = true;
        }
        else
        {
            main._spriteRen.flipX = false;
        }
        //抓取状态限制
        if (main.GetState() == ItemState.CATCH)
        {
            animator.SetBool("isCatch", true);
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        Item_kid main = GetKidMain(animator);

        //main._spriteRen.color = Color.white;
    }
}
