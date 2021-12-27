using System;
using UnityEngine;

public enum BehaviourStatus
{
    INVALID,
    SUCCESS,
    FAILURE,
    RUNNING,
    ABORTED,
};

public abstract class BTBehavior
{

    public void SetGameObject(GameObject GO)
    {
        gameObject = GO;
    }

    public BehaviourStatus Tick()
    {
        if (status != BehaviourStatus.RUNNING)
            OnInit();

        status = Update();

        if (status != BehaviourStatus.RUNNING)
            OnTerminate(status);
        return status;
    }
    public void Abort()
    {
        OnTerminate(BehaviourStatus.ABORTED);
        status = BehaviourStatus.ABORTED;
    }
    public void Reset()
    {
        status = BehaviourStatus.INVALID;
    }
    public bool IsRunning()
    {
        return status == BehaviourStatus.RUNNING;
    }
    public bool IsTerminated()
    {
        return status == BehaviourStatus.SUCCESS || status == BehaviourStatus.FAILURE;
    }
    public BehaviourStatus GetStatus()
    {
        return status;
    }

    protected GameObject gameObject;

    protected abstract BehaviourStatus Update();
    protected virtual void OnInit() { }
    protected virtual void OnTerminate(BehaviourStatus status) { }

    private BehaviourStatus status;

}
