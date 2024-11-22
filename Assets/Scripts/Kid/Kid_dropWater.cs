using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid_dropWater : KidStatesBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        main._spriteRen.color = Color.red;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        if (main.GetState() == ItemState.CATCH)
        {
            animator.SetBool("isCatch", true);
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        Item_kid main = GetKidMain(animator);

        main._spriteRen.color = Color.white;
    }
}
