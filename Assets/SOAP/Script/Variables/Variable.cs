using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public abstract class Variable<T> : ScriptableObject
{
    [SerializeField, OnValueChanged("InvokeOnValueChangedEvent")] private T value;
    [SerializeField] private bool persistRuntimeChanges = false;

    public event Action<T> OnValueChanged;

    private T valueBeforePlayMode;

    public T Value
    {
        get => value;
        set
        {
            this.value = value;
            InvokeOnValueChangedEvent();
        }
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange stateChange)
    {
        if (stateChange == UnityEditor.PlayModeStateChange.EnteredPlayMode)
        {
            valueBeforePlayMode = value;
        }
        else if (stateChange == UnityEditor.PlayModeStateChange.ExitingPlayMode)
        {
            if (!persistRuntimeChanges)
                value = valueBeforePlayMode;
        }
    }
#endif

    private void InvokeOnValueChangedEvent()
    {
        OnValueChanged?.Invoke(value);
    }
}