using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSelectionService
{
    public GameEntry SelectRandomGame(IReadOnlyList<GameEntry> availableGameEntries)
    {
        if (availableGameEntries == null || availableGameEntries.Count == 0)
            throw new ArgumentException("Cannot select a game from an empty list of game entries");

        int randomGameIndex = UnityEngine.Random.Range(0, availableGameEntries.Count);
        return availableGameEntries[randomGameIndex];
    }
}
