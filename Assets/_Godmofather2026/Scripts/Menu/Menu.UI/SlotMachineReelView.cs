using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineReelView : MonoBehaviour
{
    [BoxGroup("References"), Required]
    [SerializeField] private RectTransform _reelContent;

    [BoxGroup("References"), Required]
    [SerializeField] private ReelItemView _reelItemPrefab;

    [BoxGroup("References"), Required]
    [SerializeField] private AvailableGamesRegistry _availableGamesRegistry;

    [BoxGroup("References"), Required]
    [SerializeField] private SlotSpinConfig _spinConfig;

    [BoxGroup("Disposition"), MinValue(1)]
    [SerializeField] private int _visibleItemCount = 3;

    [BoxGroup("Disposition"), MinValue(0)]
    [SerializeField] private int _bufferItemCountPerSide = 1;

    public event Action OnStopAnimationComplete;

    private enum SpinState
    {
        Idle,
        Accelerating,
        Cruising,
        Decelerating,
        Bouncing,
        Stopped
    }

    private class ActiveReelItem
    {
        public ReelItemView View;
        public RectTransform RectTransform;
        public int ConceptualIndex;
    }

    private List<GameEntry> _availableGameEntries;
    private List<ActiveReelItem> _activeItems;
    private float _stepHeight;
    private float _centerSlotY;

    private SpinState _currentSpinState = SpinState.Idle;
    private float _stateElapsedTime;
    private float _currentSpeedInStepsPerSecond;
    private float _scrollProgress;
    private float _decelerationStartProgress;
    private int _decelerationTargetProgress;
    private float _bounceRenderOffset;

    private float _spinStartTime;
    private bool _isStopRequested;
    private GameEntry _pendingTargetGameEntry;

    private void Awake()
    {
        BuildInitialLayout();
    }

    private void BuildInitialLayout()
    {
        _availableGameEntries = _availableGamesRegistry.GetAllGameEntries().ToList();

        int totalItemCount = _visibleItemCount + _bufferItemCountPerSide * 2;
        int centerItemSlot = totalItemCount / 2;

        VerticalLayoutGroup layoutGroup = _reelContent.GetComponent<VerticalLayoutGroup>();
        float itemHeight = _reelItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
        float itemSpacing = layoutGroup != null ? layoutGroup.spacing : 0f;
        _stepHeight = itemHeight + itemSpacing;
        _centerSlotY = 0f;

        _activeItems = new List<ActiveReelItem>(totalItemCount);
        for (int itemSlot = 0; itemSlot < totalItemCount; itemSlot++)
        {
            ReelItemView itemView = Instantiate(_reelItemPrefab, _reelContent);
            ActiveReelItem activeItem = new ActiveReelItem
            {
                View = itemView,
                RectTransform = itemView.GetComponent<RectTransform>(),
                ConceptualIndex = itemSlot - centerItemSlot
            };
            activeItem.View.SetData(GetCyclicGameEntry(activeItem.ConceptualIndex));
            _activeItems.Add(activeItem);
        }

        if (layoutGroup != null)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_reelContent);
            layoutGroup.enabled = false;
        }
        
        ApplyItemPositions();
    }

    public void StartSpin()
    {
        _currentSpinState = SpinState.Accelerating;
        _stateElapsedTime = 0f;
        _currentSpeedInStepsPerSecond = 0f;
        _bounceRenderOffset = 0f;
        _spinStartTime = Time.time;
        _isStopRequested = false;
        _pendingTargetGameEntry = null;
    }

    public void StopOnGame(GameEntry targetGameEntry)
    {
        bool canStopFromCurrentState = _currentSpinState == SpinState.Accelerating || _currentSpinState == SpinState.Cruising;
        if (!canStopFromCurrentState)
            return;

        _isStopRequested = true;
        _pendingTargetGameEntry = targetGameEntry;
        TryBeginDecelerationIfMinimumSpinDurationElapsed();
    }

    private void TryBeginDecelerationIfMinimumSpinDurationElapsed()
    {
        if (!_isStopRequested)
            return;

        float elapsedSpinDuration = Time.time - _spinStartTime;
        if (elapsedSpinDuration < _spinConfig.MinimumSpinDuration)
            return;

        int targetIndex = _availableGameEntries.IndexOf(_pendingTargetGameEntry);
        if (targetIndex < 0)
        {
            _availableGameEntries = _availableGamesRegistry.GetAllGameEntries().ToList();
            targetIndex = _availableGameEntries.IndexOf(_pendingTargetGameEntry);
        }

        if (targetIndex < 0)
        {
            Debug.LogError($"[SlotMachineReelView] Le jeu selectionne '{_pendingTargetGameEntry.DisplayName}' est introuvable dans le registre des jeux disponibles, le reel ne peut pas s'arreter dessus.");
            _isStopRequested = false;
            return;
        }

        int entryCount = _availableGameEntries.Count;
        int minimumExtraSteps = entryCount * 2;
        int targetProgress = Mathf.CeilToInt(_scrollProgress) + minimumExtraSteps;
        while (ProperModulo(targetProgress, entryCount) != targetIndex)
            targetProgress++;

        _decelerationStartProgress = _scrollProgress;
        _decelerationTargetProgress = targetProgress;
        _currentSpinState = SpinState.Decelerating;
        _stateElapsedTime = 0f;
        _isStopRequested = false;
    }

    private void Update()
    {
        switch (_currentSpinState)
        {
            case SpinState.Accelerating:
                UpdateAccelerating(Time.deltaTime);
                break;
            case SpinState.Cruising:
                UpdateCruising(Time.deltaTime);
                break;
            case SpinState.Decelerating:
                UpdateDecelerating(Time.deltaTime);
                break;
            case SpinState.Bouncing:
                UpdateBouncing(Time.deltaTime);
                break;
            default:
                return;
        }

        RecycleItemsOutsideViewport();
        ApplyItemPositions();
    }

    private void UpdateAccelerating(float deltaTime)
    {
        _stateElapsedTime += deltaTime;

        float accelerationDuration = Mathf.Max(_spinConfig.AccelerationDuration, 0.0001f);
        float accelerationProgress = Mathf.Clamp01(_stateElapsedTime / accelerationDuration);
        _currentSpeedInStepsPerSecond = _spinConfig.CruiseSpeedInStepsPerSecond * accelerationProgress;

        _scrollProgress += _currentSpeedInStepsPerSecond * deltaTime;

        if (accelerationProgress >= 1f)
        {
            _currentSpinState = SpinState.Cruising;
            _stateElapsedTime = 0f;
        }

        TryBeginDecelerationIfMinimumSpinDurationElapsed();
    }

    private void UpdateCruising(float deltaTime)
    {
        _currentSpeedInStepsPerSecond = _spinConfig.CruiseSpeedInStepsPerSecond;
        _scrollProgress += _currentSpeedInStepsPerSecond * deltaTime;

        TryBeginDecelerationIfMinimumSpinDurationElapsed();
    }

    private void UpdateDecelerating(float deltaTime)
    {
        _stateElapsedTime += deltaTime;

        float decelerationDuration = Mathf.Max(_spinConfig.DecelerationDuration, 0.0001f);
        float decelerationProgress = Mathf.Clamp01(_stateElapsedTime / decelerationDuration);
        float easedProgress = EaseOutCubic(decelerationProgress);

        _scrollProgress = Mathf.Lerp(_decelerationStartProgress, _decelerationTargetProgress, easedProgress);

        if (decelerationProgress >= 1f)
        {
            _scrollProgress = _decelerationTargetProgress;
            _currentSpinState = SpinState.Bouncing;
            _stateElapsedTime = 0f;
        }
    }

    private void UpdateBouncing(float deltaTime)
    {
        _stateElapsedTime += deltaTime;

        float bounceDuration = Mathf.Max(_spinConfig.BounceDuration, 0.0001f);
        float bounceProgress = Mathf.Clamp01(_stateElapsedTime / bounceDuration);

        _bounceRenderOffset = _spinConfig.BounceAmplitude * Mathf.Sin(Mathf.PI * bounceProgress) * (1f - bounceProgress);

        if (bounceProgress >= 1f)
        {
            _bounceRenderOffset = 0f;
            _currentSpinState = SpinState.Stopped;
            OnStopAnimationComplete?.Invoke();
        }
    }

    private void RecycleItemsOutsideViewport()
    {
        int maxConceptualIndex = int.MinValue;
        foreach (ActiveReelItem activeItem in _activeItems)
            maxConceptualIndex = Mathf.Max(maxConceptualIndex, activeItem.ConceptualIndex);

        float recycleThreshold = _scrollProgress - (_activeItems.Count / 2f) - 0.5f;

        foreach (ActiveReelItem activeItem in _activeItems)
        {
            if (activeItem.ConceptualIndex >= recycleThreshold)
                continue;

            maxConceptualIndex++;
            activeItem.ConceptualIndex = maxConceptualIndex;
            activeItem.View.SetData(GetCyclicGameEntry(maxConceptualIndex));
        }
    }

    private void ApplyItemPositions()
    {
        float renderedScrollProgress = _scrollProgress + _bounceRenderOffset;

        foreach (ActiveReelItem activeItem in _activeItems)
        {
            Vector2 anchoredPosition = activeItem.RectTransform.anchoredPosition;
            anchoredPosition.y = _centerSlotY + (activeItem.ConceptualIndex - renderedScrollProgress) * _stepHeight;
            activeItem.RectTransform.anchoredPosition = anchoredPosition;
        }
    }

    private GameEntry GetCyclicGameEntry(int conceptualIndex)
    {
        int entryCount = _availableGameEntries.Count;
        return _availableGameEntries[ProperModulo(conceptualIndex, entryCount)];
    }

    private static int ProperModulo(int value, int modulus)
    {
        int result = value % modulus;
        return result < 0 ? result + modulus : result;
    }

    private static float EaseOutCubic(float progress)
    {
        float inverseProgress = 1f - progress;
        return 1f - inverseProgress * inverseProgress * inverseProgress;
    }
}
