using NaughtyAttributes;
using UnityEngine;

public class ChronoTrack : MonoBehaviour
{
    [BoxGroup("References"), Required]
    [SerializeField] private Chronometer _chronometer;

    [BoxGroup("References"), Required]
    [SerializeField] private PlayerInputHandler _inputHandler;

    [BoxGroup("Display"), Required]
    [SerializeField] private FloatVariable _elapsedTimeVariable;

    [BoxGroup("Display"), Required]
    [SerializeField] private FloatVariable _chronoVisibilityAlphaVariable;

    [BoxGroup("Debug"), ReadOnly]
    [SerializeField] private int _trackIndex;

    [BoxGroup("Debug"), ReadOnly]
    [SerializeField] private int _assignedPlayerIndex;

    [BoxGroup("Debug"), ProgressBar("Progression", 1f), ReadOnly]
    [SerializeField] private float _elapsedTimeRatio;

    public Chronometer Chronometer => _chronometer;
    public int TrackIndex => _trackIndex;
    public int AssignedPlayerIndex => _assignedPlayerIndex;

    private void Awake()
    {
        _inputHandler.OnStopChronoPressed += HandleStopChronoPressed;
    }

    private void OnDestroy()
    {
        _inputHandler.OnStopChronoPressed -= HandleStopChronoPressed;
    }

    public void SetTrackIndex(int newTrackIndex)
    {
        _trackIndex = newTrackIndex;
    }

    public void AssignPlayer(int playerIndex)
    {
        _assignedPlayerIndex = playerIndex;
    }

    public void SetInputEnabled(bool isEnabled)
    {
        _inputHandler.SetInputEnabled(isEnabled);
    }

    public void UpdateDisplayState(float elapsedTime, float visibilityAlpha, float targetTime)
    {
        _elapsedTimeVariable.Value = elapsedTime;
        _chronoVisibilityAlphaVariable.Value = visibilityAlpha;
        _elapsedTimeRatio = targetTime > 0f ? Mathf.Clamp01(elapsedTime / targetTime) : 0f;
    }

    public void ResetDisplay(float targetTime)
    {
        _chronometer.ResetChrono();
        UpdateDisplayState(0f, 1f, targetTime);
    }

    private void HandleStopChronoPressed()
    {
        _chronometer.StopChrono();
    }
}
