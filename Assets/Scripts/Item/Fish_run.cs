using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_run : FishStatesBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var main = GetFishMain(animator);

        //main._runPar.Play();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        var main = GetFishMain(animator);

        float randSpeedForce = Random.Range(main._speedForce.x, main._speedForce.y);
        Quaternion randQuaternion = Quaternion.Euler(
            Random.Range(-main._angleRun.x, main._angleRun.x),
            Random.Range(-main._angleRun.y, main._angleRun.y),
            Random.Range(-main._angleRun.z, main._angleRun.z));
        Vector3 randDir = randQuaternion * Vector3.forward;
        if (main.GetState() == ItemState.WATER)
            main._rb.AddForce(randSpeedForce * main._rb.mass * randDir, ForceMode.Impulse);

        animator.SetTrigger("tIdle");
    }
}
