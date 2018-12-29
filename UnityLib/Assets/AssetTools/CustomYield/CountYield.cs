using System;
using UnityEngine;

public class CountYield : CustomYieldInstruction
{
    public int Amount { get; private set; }
    private readonly Action<float> onProgress;
    private readonly Action onComplete;
    private int completeCount;
    public int CompleteCount
    {
        get
        {
            return completeCount;
        }
        set
        {
            completeCount = value;
            if (onProgress != null)
                onProgress((float)CompleteCount / Amount);
            if (completeCount == Amount && onComplete != null)
                onComplete();
        }
    }
    public CountYield(int amount, Action<float> progress = null, Action complete = null)
    {
        Amount = amount;
        onProgress = progress;
        onComplete = complete;
    }

    public override bool keepWaiting
    {
        get
        {
            return completeCount < Amount;
        }
    }
}
