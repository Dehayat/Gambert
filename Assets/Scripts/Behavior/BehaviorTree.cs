using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree
{
    private BTBehavior rootbehaviour;
    private GameObject gameObject;

    public void SetRootBehaviour(BTBehavior behavior)
    {
        rootbehaviour = behavior;
    }
    public void SetGameObject(GameObject GO)
    {
        gameObject = GO;
        if (rootbehaviour != null)
        {
            rootbehaviour.SetGameObject(gameObject);
        }
        else
        {
            Debug.LogWarning("Assigning GameObject with no root behaviour");
        }
    }
    public void Update()
    {
        if (rootbehaviour != null)
            rootbehaviour.Tick();
    }
}
