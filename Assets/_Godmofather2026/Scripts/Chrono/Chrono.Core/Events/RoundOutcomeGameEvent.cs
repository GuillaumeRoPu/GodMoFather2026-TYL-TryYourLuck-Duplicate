using UnityEngine;

[CreateAssetMenu(menuName = "Game Events/Chrono/Round Outcome")]
public class RoundOutcomeGameEvent : GameEvent<int, bool, float>
{
    [SerializeField] private IntReference _playerIndexReference;
    [SerializeField] private BoolReference _successReference;
    [SerializeField] private FloatReference _deltaReference;

    protected override int GetArg0() => _playerIndexReference.Value;
    protected override bool GetArg1() => _successReference.Value;
    protected override float GetArg2() => _deltaReference.Value;
}
