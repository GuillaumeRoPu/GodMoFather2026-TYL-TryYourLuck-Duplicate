using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [BoxGroup("References"), Required]
    [SerializeField] private CountdownSequence _countdownSequence;

    [BoxGroup("Manches"), ValidateInput("HasAtLeastOneRound", "Ajoutez au moins une manche")]
    [SerializeField] private SerializedDictionary<int, RoundConfig> _roundConfigsByIndex;
    
    [BoxGroup("Manches")]
    [SerializeField] private float _timeBetweenRounds = 3;
    private Coroutine _countdownSequenceCoroutine;

    [BoxGroup("Channels"), Required]
    [SerializeField] private ChronoStoppedGameEvent _onChronoStoppedChannel;

    [BoxGroup("Channels"), Required]
    [SerializeField] private RoundOutcomeGameEvent _onRoundOutcomeChannel;

    [BoxGroup("Channels"), Required]
    [SerializeField] private IntGameEvent _onRoundCompletedChannel;
    
    [BoxGroup("Channels"), Required]
    [SerializeField] private VoidGameEvent _onGameCompletedChannel;

    [BoxGroup("Channels"), Required]
    [SerializeField] private IntGameEvent _onTurnChangedChannel;

    [BoxGroup("Channels"), Required]
    [SerializeField] private IntGameEvent _onCountdownTickChannel;

    [BoxGroup("Channels"), Required]
    [SerializeField] private VoidGameEvent _onCountdownFinishedChannel;

    [BoxGroup("Variables"), Required]
    [SerializeField] private IntVariable _currentRoundIndexVariable;

    [BoxGroup("Variables"), Required]
    [SerializeField] private FloatVariable _currentTargetTimeVariable;

    [BoxGroup("Debug"), ReadOnly]
    [SerializeField] private int _currentRoundIndex;
    

    public int RoundCount => _roundConfigsByIndex.Count;

    private IRoundOutcomeRule _outcomeRule;
    private ITurnProvider _turnProvider;
    private IReadOnlyList<ChronoTrack> _chronoTracks;
    private IChronoVisibility _currentVisibility;

    public void Initialize(IRoundOutcomeRule outcomeRule, ITurnProvider turnProvider, IReadOnlyList<ChronoTrack> chronoTracks)
    {
        _outcomeRule = outcomeRule;
        _turnProvider = turnProvider;
        _chronoTracks = chronoTracks;

        for (int trackIndex = 0; trackIndex < chronoTracks.Count; trackIndex++)
        {
            ChronoTrack track = chronoTracks[trackIndex];
            track.SetTrackIndex(trackIndex);
            track.Chronometer.OnTick += elapsedTime => HandleChronoTick(track, elapsedTime);
            track.Chronometer.OnStopped += stoppedTime => HandleChronoStopped(track, stoppedTime);
        }

        _countdownSequence.OnCountdownTick += HandleCountdownTick;
        _countdownSequence.OnCountdownFinished += HandleCountdownFinished;
    }

    public void StartGame()
    {
        _currentRoundIndex = 0;
        _currentRoundIndexVariable.Value = _currentRoundIndex;
        BeginRound(0);
    }

    private bool HasAtLeastOneRound(SerializedDictionary<int, RoundConfig> value)
    {
        return value != null && value.Count > 0;
    }

    private void BeginRound(float cooldown)
    {
        RoundConfig roundConfig = _roundConfigsByIndex[_currentRoundIndex];
        _currentTargetTimeVariable.Value = roundConfig.TargetTime;
        _currentVisibility = ChronoVisibilityFactory.Create(roundConfig.VisibilityType);

        foreach (ChronoTrack track in _chronoTracks)
            track.ResetDisplay(_currentTargetTimeVariable.Value);

        _turnProvider.BeginRound();
        
        if (_countdownSequenceCoroutine != null)
        {
            StopCoroutine(_countdownSequenceCoroutine);
            _countdownSequenceCoroutine = null;
        }
        _countdownSequenceCoroutine = StartCoroutine(BeginTurn(cooldown));
    }

    private IEnumerator BeginTurn(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        _countdownSequence.PlayCountdown(3);
    }

    private void HandleCountdownTick(int remainingSeconds)
    {
        _onCountdownTickChannel.Raise(remainingSeconds);
    }

    private void HandleCountdownFinished()
    {
        _onCountdownFinishedChannel.Raise();
        ActivateCurrentTracks();
    }

    private void ActivateCurrentTracks()
    {
        foreach (ChronoTrack track in _chronoTracks)
            track.SetInputEnabled(false);

        IReadOnlyList<int> activeTrackIndices = _turnProvider.GetActiveTrackIndices();
        foreach (int trackIndex in activeTrackIndices)
        {
            ChronoTrack track = _chronoTracks[trackIndex];
            int playerIndex = _turnProvider.GetPlayerIndexForTrack(trackIndex);
            track.AssignPlayer(playerIndex);
            track.SetInputEnabled(true);
            track.Chronometer.ResumeChrono();
            _onTurnChangedChannel.Raise(playerIndex);
        }
    }

    private void HandleChronoTick(ChronoTrack track, float elapsedTime)
    {
        ChronoVisibilityState visibilityState = _currentVisibility.Evaluate(elapsedTime, _currentTargetTimeVariable.Value);
        track.UpdateDisplayState(elapsedTime, visibilityState.alpha, _currentTargetTimeVariable.Value);
    }

    private void HandleChronoStopped(ChronoTrack track, float stoppedTime)
    {
        track.SetInputEnabled(false);
        int playerIndex = track.AssignedPlayerIndex;
        _onChronoStoppedChannel.Raise(playerIndex, stoppedTime);

        RoundOutcome outcome = _outcomeRule.Evaluate(stoppedTime, _currentTargetTimeVariable.Value);
        _onRoundOutcomeChannel.Raise(playerIndex, outcome.success, outcome.delta);

        track.UpdateDisplayState(stoppedTime, 1f, _currentTargetTimeVariable.Value);

        _turnProvider.NotifyTrackStopped(track.TrackIndex, outcome);

        if (_turnProvider.IsRoundComplete())
            CompleteRound();
        else if (!IsAnyOtherTrackRunning(track))
        {
            if (_countdownSequenceCoroutine != null)
            {
                StopCoroutine(_countdownSequenceCoroutine);
                _countdownSequenceCoroutine = null;
            }
            _countdownSequenceCoroutine = StartCoroutine(BeginTurn(_timeBetweenRounds));
        }
            
    }

    private bool IsAnyOtherTrackRunning(ChronoTrack excludedTrack)
    {
        foreach (ChronoTrack track in _chronoTracks)
        {
            if (track != excludedTrack && track.Chronometer.IsRunning)
                return true;
        }
        return false;
    }

    private void CompleteRound()
    {
        foreach (ChronoTrack track in _chronoTracks)
        {
            track.SetInputEnabled(false);
            track.Chronometer.ResetChrono();
        }

        _onRoundCompletedChannel.Raise(_currentRoundIndex);


        _currentRoundIndex++;
        _currentRoundIndexVariable.Value = _currentRoundIndex;

        if (_currentRoundIndex < _roundConfigsByIndex.Count)
            BeginRound(_timeBetweenRounds);
        else
            _onGameCompletedChannel?.Raise();
    }
}
