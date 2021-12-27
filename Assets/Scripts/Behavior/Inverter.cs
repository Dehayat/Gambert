using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : BTBehavior
{
    public void SetBehaviour(BTBehavior behavior)
    {
        childBehavior = behavior;
    }
    protected override BehaviourStatus Update()
    {
        childBehavior.SetGameObject(gameObject);
        var status = childBehavior.Tick();
        if (status == BehaviourStatus.FAILURE)
            return BehaviourStatus.SUCCESS;
        if (status == BehaviourStatus.SUCCESS)
            return BehaviourStatus.FAILURE;
        return status;
    }
    protected override void OnTerminate(BehaviourStatus status)
    {
        if (childBehavior.IsRunning())
            childBehavior.Abort();
    }
    private BTBehavior childBehavior;
}
