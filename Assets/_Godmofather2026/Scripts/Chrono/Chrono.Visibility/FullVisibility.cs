public class FullVisibility : IChronoVisibility
{
    public ChronoVisibilityState Evaluate(float elapsedTime, float targetTime)
    {
        return new ChronoVisibilityState(1f);
    }
}
