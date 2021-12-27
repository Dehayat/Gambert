using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemybz : MonoBehaviour
{
    private BehaviorTree bt;
    private void Awake()
    {
        SetupBT();
    }

    private void SetupBT()
    {
        bt = new BehaviorTree();
        bt.SetRootBehaviour(new bzIdle());
        bt.SetGameObject(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        bt.Update();
    }

    public class bzIdle : BTBehavior
    {
        protected override void OnInit()
        {
            Debug.Log("Idling now");
        }
        protected override BehaviourStatus Update()
        {
            return BehaviourStatus.RUNNING;
        }
        protected override void OnTerminate(BehaviourStatus status)
        {
            Debug.Log("No longer    Idling");
        }
    }

}
