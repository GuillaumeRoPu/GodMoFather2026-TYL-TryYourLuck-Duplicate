using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [BoxGroup("Input"), Required]
    [SerializeField] private InputActionReference _stopChronoAction;

    [BoxGroup("Debug"), ReadOnly]
    [SerializeField] private bool _isInputEnabled = true;

    public event Action OnStopChronoPressed;

    private void OnEnable()
    {
        _stopChronoAction.action.Enable();
        _stopChronoAction.action.performed += HandleStopChronoActionPerformed;
    }

    private void OnDisable()
    {
        _stopChronoAction.action.performed -= HandleStopChronoActionPerformed;
        _stopChronoAction.action.Disable();
    }

    public void SetInputEnabled(bool isEnabled)
    {
        _isInputEnabled = isEnabled;
    }

    private void HandleStopChronoActionPerformed(InputAction.CallbackContext context)
    {
        if (!_isInputEnabled)
            return;

        OnStopChronoPressed?.Invoke();
    }
}
