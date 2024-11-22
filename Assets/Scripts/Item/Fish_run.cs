using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_run : FishStatesBase
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        var fish = GetFishMain(animator);

        float randSpeedForce = Random.Range(fish._speedForce.x, fish._speedForce.y);
        Quaternion randQuaternion = Quaternion.Euler(
            Random.Range(-fish._angleRun.x, fish._angleRun.x),
            Random.Range(-fish._angleRun.y, fish._angleRun.y),
            Random.Range(-fish._angleRun.z, fish._angleRun.z));
        Vector3 randDir = randQuaternion * Vector3.forward;
        if (fish.GetState() == ItemState.WATER)
            fish._rb.AddForce(randSpeedForce * fish._rb.mass * randDir, ForceMode.Impulse);

        animator.SetTrigger("tIdle");
    }
}
