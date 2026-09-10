using NaughtyAttributes;
using UnityEngine;

public class ChronoClosestGameController : MonoBehaviour
{
    [BoxGroup("References"), Required]
    [SerializeField] private RoundManager _roundManager;

    [BoxGroup("References"), Required]
    [SerializeField] private ChronoTrack _playerOneTrack;

    [BoxGroup("References"), Required]
    [SerializeField] private ChronoTrack _playerTwoTrack;

    [BoxGroup("Debug"), ReadOnly]
    [SerializeField] private float _playerOneDelta = float.MaxValue;

    [BoxGroup("Debug"), ReadOnly]
    [SerializeField] private float _playerTwoDelta = float.MaxValue;

    [BoxGroup("Debug"), ReadOnly]
    [SerializeField] private int _roundWinnerPlayerIndex = -1;

    [BoxGroup("Debug"), ReadOnly]
    [SerializeField] private int _roundsWonByPlayerOne;

    [BoxGroup("Debug"), ReadOnly]
    [SerializeField] private int _roundsWonByPlayerTwo;

    private void Start()
    {
        ChronoTrack[] chronoTracks = { _playerOneTrack, _playerTwoTrack };
        _roundManager.Initialize(new ClosestToTargetRule(), new SinglePlayerTurnProvider(chronoTracks.Length), chronoTracks);

        _roundManager.StartGame();
    }

    private void OnDestroy()
    {

    }

    public void HandleRoundOutcome(int playerIndex, bool success, float delta)
    {
        if (playerIndex == 0)
            _playerOneDelta = delta;
        else
            _playerTwoDelta = delta;
    }

    public void HandleRoundCompleted(int roundIndex)
    {
        _roundWinnerPlayerIndex = _playerOneDelta <= _playerTwoDelta ? 0 : 1;

        if (_roundWinnerPlayerIndex == 0)
            _roundsWonByPlayerOne++;
        else
            _roundsWonByPlayerTwo++;

        Debug.Log($"[ChronoClosest] Manche {roundIndex + 1} terminee : Joueur {_roundWinnerPlayerIndex + 1} gagne la manche (delta P1: {_playerOneDelta:F3}s, delta P2: {_playerTwoDelta:F3}s).");

        _playerOneDelta = float.MaxValue;
        _playerTwoDelta = float.MaxValue;
    }

    public void HandleGameCompleted()
    {
        int gameWinnerPlayerIndex = _roundsWonByPlayerOne >= _roundsWonByPlayerTwo ? 0 : 1;
        Debug.Log($"[ChronoClosest] Fin de la partie (BO3) : Joueur {gameWinnerPlayerIndex + 1} remporte le jeu ({_roundsWonByPlayerOne} - {_roundsWonByPlayerTwo}).");
    }
}
