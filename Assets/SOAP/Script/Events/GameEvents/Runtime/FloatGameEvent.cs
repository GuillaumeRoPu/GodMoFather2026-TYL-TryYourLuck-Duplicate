using UnityEngine;

[CreateAssetMenu(menuName = "Game Events/Float")]
public class FloatGameEvent : GameEvent<float>
{
    [SerializeField] private FloatReference reference0;
    protected override float GetArg0() => reference0.Value;
}