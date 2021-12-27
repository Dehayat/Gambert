using System.Collections.Generic;

public class Filter : Sequence
{
    public void AddCondition(BTBehavior behavior)
    {
        behaviors.Insert(0, behavior);
    }
    public void AddAction(BTBehavior behavior)
    {
        AddBehavior(behavior);
    }
}
