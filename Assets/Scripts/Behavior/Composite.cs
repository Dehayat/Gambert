using System.Collections.Generic;

public abstract class Composite : BTBehavior
{
    public void AddBehavior(BTBehavior behavior)
    {
        behaviors.Add(behavior);
    }
    public void ClearBehaviors()
    {
        behaviors.Clear();
    }

    protected List<BTBehavior> behaviors = new List<BTBehavior>();
}
