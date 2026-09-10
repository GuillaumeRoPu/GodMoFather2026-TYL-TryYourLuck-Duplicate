using System;

[Serializable]
public struct RoundOutcomeData
{
    public int roundIndex;
    public int winningPlayerIndex;
    public bool isTie;

    public RoundOutcomeData(int roundIndex, int winningPlayerIndex, bool isTie)
    {
        this.roundIndex = roundIndex;
        this.winningPlayerIndex = winningPlayerIndex;
        this.isTie = isTie;
    }
}
