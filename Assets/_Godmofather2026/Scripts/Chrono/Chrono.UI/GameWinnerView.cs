using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class GameWinnerView : MonoBehaviour
{
    [BoxGroup("References"), Required]
    [SerializeField] private TMP_Text _winnerLabel;

    public void SetWinnerPlayerIndex(int winnerPlayerIndex)
    {
        _winnerLabel.text = $"Player {winnerPlayerIndex + 1}";
    }
}
