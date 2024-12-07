using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid_sitDown : KidStatesBase
{
    float _currentSitDownTime = 0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        _currentSitDownTime = 0;
        //main._spriteRen.color = Color.blue;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        //Hunger
        if (main._isHunger)
        {
            animator.SetTrigger("tHunger");
        }
        //SitDonw Time
        _currentSitDownTime += Time.deltaTime;
        if (_currentSitDownTime > main._sitDownTime)
        {
            animator.SetTrigger("tIdle");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        //main._spriteRen.color = Color.white;
    }
}
