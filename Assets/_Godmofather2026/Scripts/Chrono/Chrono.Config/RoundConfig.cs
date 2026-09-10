using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Round Config", menuName = "Chrono/Round Config")]
public class RoundConfig : ScriptableObject
{
    [SerializeField, BoxGroup("Regles")] private bool _randomizeTime = true;

    [BoxGroup("Regles"), ValidateInput("IsTargetTimeValid", "Le temps cible doit etre superieur a zero")]
    [SerializeField, Label("Target Time"), HideIf("_randomizeTime")] private float _targetTime = 5f;


    [BoxGroup("Regles"), SerializeField, Label("Target Time"), ShowIf("_randomizeTime"), ValidateInput("IsRandomRangeTimeValid", "Le temps cible doit etre compris entre 1 et l'infini")]
    private Vector2 _targetTimeRange = new Vector2(1f, 5f);

    [BoxGroup("Regles")]
    [SerializeField] private VisibilityType _visibilityType = VisibilityType.Full;

    public float TargetTime => _randomizeTime ? Random.Range(_targetTimeRange.x, _targetTimeRange.y) : _targetTime;
    public VisibilityType VisibilityType => _visibilityType;

    private bool IsTargetTimeValid(float value)
    {
        return value > 0f;
    }

    private bool IsRandomRangeTimeValid(Vector2 value)
    {
        return value.x > 0 && value.y >= value.x;
    }
}
