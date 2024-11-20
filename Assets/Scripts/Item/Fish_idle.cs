using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_idle : FishStatesBase
{
    float _currentIdleTime = 0;
    float _randIdleTime = 0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        var fish = GetFishMain(animator);

        _currentIdleTime = 0;
        _randIdleTime = Random.Range(fish._idleTime.x, fish._idleTime.y);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

        _currentIdleTime += Time.deltaTime;
        if (_currentIdleTime >= _randIdleTime)
        {
            animator.SetTrigger("tRun");
        }
    }
}
