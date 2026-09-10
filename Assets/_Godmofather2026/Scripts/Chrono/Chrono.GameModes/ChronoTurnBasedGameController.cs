using NaughtyAttributes;
using UnityEngine;

public class ChronoTurnBasedGameController : MonoBehaviour
{
    [BoxGroup("References"), Required]
    [SerializeField] private RoundManager _roundManager;

    [BoxGroup("References"), Required]
    [SerializeField] private ChronoTrack _sharedTrack;

    [BoxGroup("Regles")]
    [SerializeField] private int _playerCount = 2;

    [BoxGroup("Channels"), Required]
    [SerializeField] private RoundOutcomeDataGameEvent _onRoundOutcomeAnnouncedChannel;

    [BoxGroup("Debug"), ReadOnly]
    [SerializeField] private int _roundsWonByPlayerOne;

    [BoxGroup("Debug"), ReadOnly]
    [SerializeField] private int _roundsWonByPlayerTwo;

    private int _lastRoundWinnerPlayerIndex = -1;

    private void Start()
    {
        ChronoTrack[] chronoTracks = { _sharedTrack };
        _roundManager.Initialize(new DoNotExceedRule(), new AlternatingTurnProvider(_playerCount), chronoTracks, autoEliminateOnExceedTarget: true, useCooldownBetweenRounds: false, IsGameOver);
        _roundManager.StartGame();
    }

    private bool IsGameOver()
    {
        return _roundsWonByPlayerOne >= 2 || _roundsWonByPlayerTwo >= 2;
    }

    private void OnDestroy()
    {
        
    }

    public void HandleRoundOutcome(int playerIndex, bool success, float delta)
    {
        if (success)
            return;

        int winnerPlayerIndex = playerIndex == 0 ? 1 : 0;
        _lastRoundWinnerPlayerIndex = winnerPlayerIndex;
        if (winnerPlayerIndex == 0)
            _roundsWonByPlayerOne++;
        else
            _roundsWonByPlayerTwo++;
    }

    public void HandleRoundCompleted(int roundIndex)
    {
        Debug.Log($"[ChronoTurnBased] Manche {roundIndex + 1} terminee : Joueur {_lastRoundWinnerPlayerIndex + 1} gagne la manche.");

        _onRoundOutcomeAnnouncedChannel.Raise(new RoundOutcomeData(roundIndex, _lastRoundWinnerPlayerIndex, isTie: false));
    }

    public void HandleGameCompleted()
    {
        int gameWinnerPlayerIndex = _roundsWonByPlayerOne >= _roundsWonByPlayerTwo ? 0 : 1;
        Debug.Log($"[ChronoTurnBased] Fin de la partie (BO3) : Joueur {gameWinnerPlayerIndex + 1} remporte le jeu ({_roundsWonByPlayerOne} - {_roundsWonByPlayerTwo}).");
    }
}
