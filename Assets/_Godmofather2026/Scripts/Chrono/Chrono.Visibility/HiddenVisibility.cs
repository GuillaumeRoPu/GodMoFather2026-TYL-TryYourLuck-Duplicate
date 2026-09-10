public class HiddenVisibility : IChronoVisibility
{
    public ChronoVisibilityState Evaluate(float elapsedTime, float targetTime)
    {
        return new ChronoVisibilityState(0f);
    }
}
