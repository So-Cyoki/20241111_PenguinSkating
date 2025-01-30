using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid_hunger : KidStatesBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        //如果饥饿的死亡时间低于一定值，就重新赋值，避免立马死亡的问题
        if (main._currentHungerDeadTime < main._hungerGodTime)
            main._currentHungerDeadTime = main._hungerGodTime;
        //一定范围内循环寻找附近的食物
        main.SetHungerFindFood(true);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        //饿到死亡！
        main._currentHungerDeadTime += Time.deltaTime;
        if (main._currentHungerDeadTime >= main._hungerDeadTime)
        {
            animator.SetTrigger("tDead");
        }

        //改变颜色(无法改变，颜色在动画系统中使用了)
        // float t = main._currentHungerDeadTime / main._hungerDeadTime;
        // main._spriteRen.color = Color.Lerp(Color.white, Color.red, t);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Item_kid main = GetKidMain(animator);

        //停止搜索食物
        main.SetHungerFindFood(false);
    }
}
