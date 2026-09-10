using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class FlipFlop : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool _isFlip = true;

    public UnityEvent OnFlip;

    public UnityEvent OnFlop;

    public void DoFlip()
    {
        if (_isFlip)
            OnFlip?.Invoke();
        else
            OnFlop?.Invoke();
        _isFlip = !_isFlip;
    }
}
