using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid_sitDown : KidStatesBase
{
    float _currentSitDownTime = 0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _currentSitDownTime = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        _currentSitDownTime += Time.deltaTime;
        if (_currentSitDownTime > main._sitDownTime)
        {
            animator.SetTrigger("tIdle");
        }
    }
}
