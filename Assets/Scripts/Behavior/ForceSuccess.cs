using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceSuccess : BTBehavior
{
    public void SetBehaviour(BTBehavior behavior)
    {
        childBehavior = behavior;
    }
    protected override BehaviourStatus Update()
    {
        childBehavior.SetGameObject(gameObject);
        var status = childBehavior.Tick();
        if (status == BehaviourStatus.FAILURE || status == BehaviourStatus.SUCCESS)
            return BehaviourStatus.SUCCESS;
        return status;
    }
    protected override void OnTerminate(BehaviourStatus status)
    {
        if (childBehavior.IsRunning())
            childBehavior.Abort();
    }
    private BTBehavior childBehavior;
}
