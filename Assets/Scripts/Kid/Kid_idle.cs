using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid_idle : KidStatesBase
{
    float _currentIdleTime = 0;
    float _randIdleTime = 0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        var main = GetKidMain(animator);

        _currentIdleTime = 0;
        _randIdleTime = Random.Range(main._idleTime.x, main._idleTime.y);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        var main = GetKidMain(animator);

        //Move
        _currentIdleTime += Time.deltaTime;
        if (_currentIdleTime >= _randIdleTime)
        {
            animator.SetTrigger("tMove");
        }

        //Hunger
        if (main._isHunger)
        {
            animator.SetTrigger("tHunger");
        }
    }
}
