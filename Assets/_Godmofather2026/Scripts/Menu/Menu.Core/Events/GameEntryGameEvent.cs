using UnityEngine;

[CreateAssetMenu(menuName = "Game Events/Menu/Game Entry")]
public class GameEntryGameEvent : GameEvent<GameEntry>
{
    protected override GameEntry GetArg0() => null;
}
