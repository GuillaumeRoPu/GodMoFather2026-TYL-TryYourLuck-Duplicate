using UnityEngine;

public class HalfVisibility : IChronoVisibility
{
    public ChronoVisibilityState Evaluate(float elapsedTime, float targetTime)
    {
        float halfTime = targetTime / 2f;
        if (halfTime <= 0f)
            return new ChronoVisibilityState(0f);

        float alpha = 1f - Mathf.Clamp01(elapsedTime / halfTime);
        return new ChronoVisibilityState(alpha);
    }
}
