public class DoNotExceedRule : IRoundOutcomeRule
{
    public RoundOutcome Evaluate(float stoppedTime, float targetTime)
    {
        bool success = stoppedTime <= targetTime;
        float delta = targetTime - stoppedTime;
        return new RoundOutcome(success, delta);
    }
}
