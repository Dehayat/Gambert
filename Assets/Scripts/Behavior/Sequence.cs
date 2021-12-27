using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Composite
{
    int currentIndex = -1;

    protected override void OnInit()
    {
        currentIndex = 0;
    }

    protected override BehaviourStatus Update()
    {
        while (currentIndex < behaviors.Count)
        {
            behaviors[currentIndex].SetGameObject(gameObject);
            BehaviourStatus status = behaviors[currentIndex].Tick();
            if (status != BehaviourStatus.SUCCESS)
            {
                return status;
            }
            currentIndex++;
            if (currentIndex == behaviors.Count)
            {
                currentIndex = -1;
                return BehaviourStatus.SUCCESS;
            }
        }
        currentIndex = -1;
        return BehaviourStatus.INVALID;
    }
    protected override void OnTerminate(BehaviourStatus status)
    {
        if (status == BehaviourStatus.ABORTED && currentIndex != -1)
        {
            behaviors[currentIndex].Abort();
        }
    }
}
