using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;

public class RoundPastilleIndicatorView : MonoBehaviour
{
    [BoxGroup("References"), Required]
    [SerializeField] private IntVariable _roundCountVariable;

    [BoxGroup("References"), Required]
    [SerializeField] private RoundPastilleView _pastillePrefab;

    [BoxGroup("References"), Required]
    [SerializeField] private Transform _pastilleContainer;

    [BoxGroup("Couleurs des joueurs"), ValidateInput("ArePlayerColorsValid", "Une couleur de joueur ne doit pas etre identique a la couleur d'egalite ou a la couleur neutre")]
    [SerializeField] private SerializedDictionary<int, Color> _playerColorsByIndex = new SerializedDictionary<int, Color>
    {
        { 0, Color.blue },
        { 1, Color.red }
    };

    [BoxGroup("Couleurs neutres")]
    [SerializeField] private Color _tieColor = Color.yellow;

    [BoxGroup("Couleurs neutres")]
    [SerializeField] private Color _unplayedColor = Color.white;

    [BoxGroup("Debug"), ReadOnly]
    [SerializeField] private int _generatedPastilleCount;

    private readonly List<RoundPastilleView> _pastilleViews = new List<RoundPastilleView>();

    private void OnEnable()
    {
        _roundCountVariable.OnValueChanged += HandleRoundCountChanged;
        HandleRoundCountChanged(_roundCountVariable.Value);
    }

    private void OnDisable()
    {
        _roundCountVariable.OnValueChanged -= HandleRoundCountChanged;
    }

    public void HandleRoundOutcomeAnnounced(RoundOutcomeData roundOutcomeData)
    {
        if (roundOutcomeData.roundIndex < 0 || roundOutcomeData.roundIndex >= _pastilleViews.Count)
            return;

        Color resultColor = roundOutcomeData.isTie ? _tieColor : _playerColorsByIndex[roundOutcomeData.winningPlayerIndex];
        _pastilleViews[roundOutcomeData.roundIndex].ApplyResultColor(resultColor);
    }

    private void HandleRoundCountChanged(int roundCount)
    {
        ClearExistingPastilles();

        for (int roundIndex = 0; roundIndex < roundCount; roundIndex++)
        {
            RoundPastilleView pastilleView = Instantiate(_pastillePrefab, _pastilleContainer);
            pastilleView.SetUnplayedColor(_unplayedColor);
            _pastilleViews.Add(pastilleView);
        }

        _generatedPastilleCount = _pastilleViews.Count;
    }

    private void ClearExistingPastilles()
    {
        for (int childIndex = _pastilleContainer.childCount - 1; childIndex >= 0; childIndex--)
            Destroy(_pastilleContainer.GetChild(childIndex).gameObject);

        _pastilleViews.Clear();
    }

    private bool ArePlayerColorsValid(SerializedDictionary<int, Color> playerColorsByIndex)
    {
        if (playerColorsByIndex == null)
            return true;

        foreach (Color playerColor in playerColorsByIndex.Values)
        {
            if (playerColor == _tieColor || playerColor == _unplayedColor)
                return false;
        }

        return true;
    }
}
