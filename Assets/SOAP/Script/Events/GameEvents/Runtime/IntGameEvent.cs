using UnityEngine;

[CreateAssetMenu(menuName = "Game Events/Int")]
public class IntGameEvent : GameEvent<int>
{
    [SerializeField] private IntReference reference0;
    protected override int GetArg0() => reference0.Value;
}