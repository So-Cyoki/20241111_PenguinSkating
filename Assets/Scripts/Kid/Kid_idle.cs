using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid_idle : KidStatesBase
{
    float _currentIdleTime = 0;
    float _randIdleTime = 0;
    float _currentHungerTime = 0;
    float _randHungerTime = 0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        var main = GetKidMain(animator);

        _currentIdleTime = 0;
        _randIdleTime = Random.Range(main._idleTime.x, main._idleTime.y);
        _randHungerTime = Random.Range(main._hungerTime.x, main._hungerTime.y);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //Move
        _currentIdleTime += Time.deltaTime;
        if (_currentIdleTime >= _randIdleTime)
        {
            animator.SetTrigger("tMove");
        }

        //Hunger
        // _currentHungerTime += Time.deltaTime;
        // if (_currentHungerTime >= _randHungerTime)
        // {
        //     animator.SetTrigger("tHunger");
        //      _currentHungerTime = 0;
        // }
    }
}
