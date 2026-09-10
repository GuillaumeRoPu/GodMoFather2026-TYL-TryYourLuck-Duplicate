using MoreMountains.Feedbacks;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class TurnIndicatorView : MonoBehaviour
{
    [BoxGroup("Mode")]
    [SerializeField] private bool _isTurnBasedMode;

    [BoxGroup("References"), Required]
    [SerializeField] private TMP_Text _activePlayerLabel;

    [BoxGroup("Feedbacks"), ShowIf("_isTurnBasedMode")]
    [SerializeField] private MMF_Player _handoffFeedback;

    private void Awake()
    {
        gameObject.SetActive(_isTurnBasedMode);
    }

    public void SetActivePlayer(int playerIndex)
    {
        _activePlayerLabel.text = "Joueur " + (playerIndex + 1);
        _handoffFeedback?.PlayFeedbacks();
    }
}
