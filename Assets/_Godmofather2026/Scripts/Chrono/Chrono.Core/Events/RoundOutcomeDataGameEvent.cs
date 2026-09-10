using UnityEngine;

[CreateAssetMenu(menuName = "Game Events/Chrono/Round Outcome Data")]
public class RoundOutcomeDataGameEvent : GameEvent<RoundOutcomeData>
{
    protected override RoundOutcomeData GetArg0() => default;
}
