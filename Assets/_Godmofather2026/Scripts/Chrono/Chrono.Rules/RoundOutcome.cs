using System;

[Serializable]
public struct RoundOutcome
{
    public bool success;
    public float delta;

    public RoundOutcome(bool success, float delta)
    {
        this.success = success;
        this.delta = delta;
    }
}
