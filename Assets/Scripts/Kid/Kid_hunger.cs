using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid_hunger : KidStatesBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        //main._spriteRen.color = Color.green;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        //main._spriteRen.color = Color.white;
    }
}
