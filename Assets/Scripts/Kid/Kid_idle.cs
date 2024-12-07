using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid_idle : KidStatesBase
{
    float _currentIdleTime = 0;
    float _randIdleTime = 0;
    bool _isTakeSlowly;//是否已经减速了
    //bool _isTakeHunger;//是否已经开始饿了

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        var main = GetKidMain(animator);

        _currentIdleTime = 0;
        _randIdleTime = Random.Range(main._idleTime.x, main._idleTime.y);
        _isTakeSlowly = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        var main = GetKidMain(animator);

        //检查前方是否还在冰块上的射线检测
        if (!Physics.Raycast(main.transform.TransformPoint(main._originOffset), main.transform.TransformDirection(main._rayDir), main._rayLength) && !_isTakeSlowly)
        {
            //Debug.Log("减速");
            main._rb.velocity *= main._slowlyMultiply;
            _isTakeSlowly = true;
        }
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
