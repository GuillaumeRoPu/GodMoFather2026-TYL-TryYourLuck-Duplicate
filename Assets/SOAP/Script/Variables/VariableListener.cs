using UnityEngine;
using UnityEngine.Events;

public abstract class VariableListener<T, TVariable> : MonoBehaviour where TVariable : Variable<T>
{
    [SerializeField] private TVariable _variable;
    public UnityEvent<T> Response;

    private void OnEnable()
    {
        if (_variable == null)
            return;

        _variable.OnValueChanged += HandleValueChanged;
        HandleValueChanged(_variable.Value);
    }

    private void OnDisable()
    {
        if (_variable == null)
            return;

        _variable.OnValueChanged -= HandleValueChanged;
    }

    private void HandleValueChanged(T value)
    {
        Response?.Invoke(value);
    }
}
