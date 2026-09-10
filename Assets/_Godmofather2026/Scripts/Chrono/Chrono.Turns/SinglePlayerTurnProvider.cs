using System.Collections.Generic;

public class SinglePlayerTurnProvider : ITurnProvider
{
    private readonly int _trackCount;
    private readonly HashSet<int> _stoppedTrackIndices = new HashSet<int>();

    public SinglePlayerTurnProvider(int trackCount)
    {
        _trackCount = trackCount;
    }

    public void BeginRound()
    {
        _stoppedTrackIndices.Clear();
    }

    public IReadOnlyList<int> GetActiveTrackIndices()
    {
        List<int> activeTrackIndices = new List<int>();
        for (int trackIndex = 0; trackIndex < _trackCount; trackIndex++)
        {
            if (!_stoppedTrackIndices.Contains(trackIndex))
                activeTrackIndices.Add(trackIndex);
        }
        return activeTrackIndices;
    }

    public int GetPlayerIndexForTrack(int trackIndex)
    {
        return trackIndex;
    }

    public void NotifyTrackStopped(int trackIndex, RoundOutcome outcome)
    {
        _stoppedTrackIndices.Add(trackIndex);
    }

    public bool IsRoundComplete()
    {
        return _stoppedTrackIndices.Count >= _trackCount;
    }
}
