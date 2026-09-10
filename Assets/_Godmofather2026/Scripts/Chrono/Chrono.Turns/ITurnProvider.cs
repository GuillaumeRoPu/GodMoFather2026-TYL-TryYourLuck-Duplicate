using System.Collections.Generic;

public interface ITurnProvider
{
    void BeginRound();
    IReadOnlyList<int> GetActiveTrackIndices();
    int GetPlayerIndexForTrack(int trackIndex);
    void NotifyTrackStopped(int trackIndex, RoundOutcome outcome);
    bool IsRoundComplete();
}
