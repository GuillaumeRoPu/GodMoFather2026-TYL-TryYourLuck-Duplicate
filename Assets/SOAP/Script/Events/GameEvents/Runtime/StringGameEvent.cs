using UnityEngine;
 
[CreateAssetMenu(menuName = "Game Events/String")]
public class StringGameEvent : GameEvent<string>
{
    [SerializeField] private StringReference reference0;
    protected override string GetArg0() => reference0.Value;
}