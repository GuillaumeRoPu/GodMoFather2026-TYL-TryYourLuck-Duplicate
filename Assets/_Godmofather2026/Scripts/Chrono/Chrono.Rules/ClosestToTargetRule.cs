using UnityEngine;

public class ClosestToTargetRule : IRoundOutcomeRule
{
    public RoundOutcome Evaluate(float stoppedTime, float targetTime)
    {
        float delta = Mathf.Abs(stoppedTime - targetTime);
        return new RoundOutcome(true, delta);
    }
}
