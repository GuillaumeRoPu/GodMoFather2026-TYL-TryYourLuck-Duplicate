using UnityEngine;

[CreateAssetMenu(menuName = "Game Events/String Float")]
public class StringFloatGameEvent : GameEvent<string, float>
{
    [SerializeField] private StringReference reference0;
    [SerializeField] private FloatReference reference1;
    
    protected override string GetArg0() => reference0.Value;
    protected override float GetArg1() => reference1.Value;
}