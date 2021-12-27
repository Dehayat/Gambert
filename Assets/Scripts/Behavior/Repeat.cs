
public class Repeat : BTBehavior
{

    public Repeat(int repeatCount)
    {
        this.repeatCount = repeatCount;
    }
    public void SetBehaviour(BTBehavior behavior)
    {
        childBehavior = behavior;
    }
    protected override void OnInit()
    {
        repeatCounter = 0;
    }
    protected override BehaviourStatus Update()
    {
        while (repeatCounter < repeatCount)
        {
            childBehavior.SetGameObject(gameObject);
            var status = childBehavior.Tick();
            if (status == BehaviourStatus.SUCCESS)
                repeatCounter++;
            else
                return status;
        }
        return BehaviourStatus.SUCCESS;
    }
    protected override void OnTerminate(BehaviourStatus status)
    {
        if (childBehavior.IsRunning())
            childBehavior.Abort();
    }

    private int repeatCount;
    private BTBehavior childBehavior;
    private int repeatCounter;

}
