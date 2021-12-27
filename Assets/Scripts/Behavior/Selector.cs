public class Selector : Composite
{
    protected override void OnInit()
    {
        currentIndex = 0;
    }
    protected override BehaviourStatus Update()
    {
        while (currentIndex < behaviors.Count)
        {
            behaviors[currentIndex].SetGameObject(gameObject);
            var status = behaviors[currentIndex].Tick();
            if (status != BehaviourStatus.FAILURE)
            {
                return status;
            }
            currentIndex++;
            if (currentIndex == behaviors.Count)
            {
                currentIndex = -1;
                return BehaviourStatus.FAILURE;
            }
        }
        return BehaviourStatus.FAILURE;
    }
    protected override void OnTerminate(BehaviourStatus status)
    {
        if (status == BehaviourStatus.ABORTED && currentIndex != -1)
        {
            behaviors[currentIndex].Abort();
        }
    }

    protected int currentIndex = -1;
}
