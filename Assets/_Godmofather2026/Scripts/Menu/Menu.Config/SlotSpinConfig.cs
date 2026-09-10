using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Slot Spin Config", menuName = "Menu/Slot Spin Config")]
public class SlotSpinConfig : ScriptableObject
{
    [BoxGroup("Timing"), MinValue(0)]
    [SerializeField] private float _accelerationDuration = 1f;

    [BoxGroup("Timing"), MinValue(0)]
    [ValidateInput("IsCruiseSpeedValid", "La vitesse de croisiere doit etre superieure a zero")]
    [SerializeField] private float _cruiseSpeedInStepsPerSecond = 8f;

    [BoxGroup("Timing"), MinValue(0)]
    [ValidateInput("IsMinimumSpinDurationValid", "La duree minimale de spin ne doit pas etre negative")]
    [SerializeField] private float _minimumSpinDuration = 5f;

    [BoxGroup("Timing"), MinValue(0)]
    [SerializeField] private float _decelerationDuration = 2f;

    [BoxGroup("Timing"), MinValue(0)]
    [SerializeField] private float _bounceAmplitude = 0.15f;

    [BoxGroup("Timing"), MinValue(0)]
    [SerializeField] private float _bounceDuration = 0.3f;

    public float AccelerationDuration => _accelerationDuration;
    public float CruiseSpeedInStepsPerSecond => _cruiseSpeedInStepsPerSecond;
    public float MinimumSpinDuration => _minimumSpinDuration;
    public float DecelerationDuration => _decelerationDuration;
    public float BounceAmplitude => _bounceAmplitude;
    public float BounceDuration => _bounceDuration;

    private bool IsCruiseSpeedValid(float value)
    {
        return value > 0f;
    }

    private bool IsMinimumSpinDurationValid(float value)
    {
        return value >= 0f;
    }
}
