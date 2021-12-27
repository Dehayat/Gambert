public enum Policy
{
    RequireOneForSuccess,
    RequireOneForFail,
}

public class Parallel : Composite
{
    public Parallel(Policy successPolicy = Policy.RequireOneForFail)
    {
        this.successPolicy = successPolicy;
    }

    protected override BehaviourStatus Update()
    {
        int terminated = 0;
        for (int i = 0; i < behaviors.Count; i++)
        {
            if (!behaviors[i].IsTerminated())
            {
                behaviors[i].SetGameObject(gameObject);
                behaviors[i].Tick();
            }
            var status = behaviors[i].GetStatus();

            if (status == BehaviourStatus.SUCCESS)
            {
                if (successPolicy == Policy.RequireOneForSuccess)
                    return BehaviourStatus.SUCCESS;
            }

            if (status == BehaviourStatus.FAILURE)
            {
                if (successPolicy == Policy.RequireOneForFail)
                    return BehaviourStatus.FAILURE;
            }
            if (behaviors[i].IsTerminated())
            {
                terminated++;
            }
            if (i == behaviors.Count - 1 && terminated != behaviors.Count)
            {
                return BehaviourStatus.RUNNING;
            }
        }
        if (successPolicy == Policy.RequireOneForSuccess)
            return BehaviourStatus.FAILURE;
        else
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

    private Policy successPolicy;
}
