using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSelector : Selector
{
    protected override void OnInit()
    {
        currentIndex = behaviors.Count;
    }
    protected override BehaviourStatus Update()
    {
        int lastActiveIndex = currentIndex;
        base.OnInit();
        var status = base.Update();
        if (lastActiveIndex < behaviors.Count && currentIndex != lastActiveIndex)
            behaviors[lastActiveIndex].Abort();
        return status;
    }
}
