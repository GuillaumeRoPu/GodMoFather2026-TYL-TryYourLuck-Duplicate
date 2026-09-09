using System.Collections.Generic;

public class AlternatingTurnProvider : ITurnProvider
{
    private static readonly List<int> _sharedTrackIndex = new List<int> { 0 };

    private readonly int _playerCount;
    private bool _isRoundComplete;

    public int CurrentPlayerIndex { get; private set; }

    public AlternatingTurnProvider(int playerCount)
    {
        _playerCount = playerCount;
    }

    public void BeginRound()
    {
        CurrentPlayerIndex = 0;
        _isRoundComplete = false;
    }

    public IReadOnlyList<int> GetActiveTrackIndices()
    {
        return _sharedTrackIndex;
    }

    public int GetPlayerIndexForTrack(int trackIndex)
    {
        return CurrentPlayerIndex;
    }

    public void NotifyTrackStopped(int trackIndex, RoundOutcome outcome)
    {
        if (!outcome.success)
        {
            _isRoundComplete = true;
            return;
        }
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _playerCount;
    }

    public bool IsRoundComplete()
    {
        return _isRoundComplete;
    }
}
