using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : Composite
{

    public void AddCondition(BTBehavior behavior)
    {
        behaviors.Insert(0, behavior);
    }
    public void AddAction(BTBehavior behavior)
    {
        AddBehavior(behavior);
    }

    protected override BehaviourStatus Update()
    {
        for (int i = 0; i < behaviors.Count; i++)
        {
            behaviors[i].SetGameObject(gameObject);
            var status = behaviors[i].Tick();

            if (status != BehaviourStatus.SUCCESS)
            {
                return status;
            }
        }
        return BehaviourStatus.SUCCESS;
    }
    protected override void OnTerminate(BehaviourStatus status)
    {
        for (int i = 0; i < behaviors.Count; i++)
        {
            if (behaviors[i].IsRunning())
            {
                behaviors[i].Abort();
            }
            behaviors[i].Reset();
        }
    }
}
