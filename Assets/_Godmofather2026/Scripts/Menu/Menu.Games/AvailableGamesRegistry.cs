using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Available Games Registry", menuName = "Menu/Available Games Registry")]
public class AvailableGamesRegistry : ScriptableObject
{
    [ValidateInput("AreGameEntriesValid", "Le registre ne doit pas etre vide et chaque identifiant doit correspondre au GameId de son GameEntry")]
    [SerializeField] private SerializedDictionary<string, GameEntry> _gameEntriesById;

    public IReadOnlyList<GameEntry> GetAllGameEntries()
    {
        return _gameEntriesById.Values.ToList();
    }

    private bool AreGameEntriesValid(SerializedDictionary<string, GameEntry> gameEntriesById)
    {
        if (gameEntriesById == null || gameEntriesById.Count == 0)
            return false;

        foreach (KeyValuePair<string, GameEntry> gameEntryById in gameEntriesById)
        {
            if (gameEntryById.Value == null)
                return false;

            if (gameEntryById.Value.GameId != gameEntryById.Key)
                return false;
        }

        return true;
    }
}
