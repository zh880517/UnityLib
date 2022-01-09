using UnityEngine;

public abstract class InitializeRequest : CustomYieldInstruction
{
    private System.Action onFinish;
    public event System.Action OnFinish
    {
        add
        {
            if (!keepWaiting)
            {
                value();
            }
            else
            {
                onFinish += value;
            }
        }
        remove
        {
            onFinish += value;
        }
    }

    public void Finished()
    {
        onFinish?.Invoke();
    }
}
