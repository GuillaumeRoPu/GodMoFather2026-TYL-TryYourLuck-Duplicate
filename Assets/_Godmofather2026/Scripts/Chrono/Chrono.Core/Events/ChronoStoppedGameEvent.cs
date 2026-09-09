using UnityEngine;

[CreateAssetMenu(menuName = "Game Events/Chrono/Chrono Stopped")]
public class ChronoStoppedGameEvent : GameEvent<int, float>
{
    [SerializeField] private IntReference _playerIndexReference;
    [SerializeField] private FloatReference _stoppedTimeReference;

    protected override int GetArg0() => _playerIndexReference.Value;
    protected override float GetArg1() => _stoppedTimeReference.Value;
}
