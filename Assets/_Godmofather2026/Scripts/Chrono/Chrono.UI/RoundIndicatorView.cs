using MoreMountains.Feedbacks;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class RoundIndicatorView : MonoBehaviour
{
    [BoxGroup("References"), Required]
    [SerializeField] private TMP_Text _roundLabel;

    [BoxGroup("Feedbacks")]
    [SerializeField] private MMF_Player _successFeedback;

    [BoxGroup("Feedbacks")]
    [SerializeField] private MMF_Player _failureFeedback;

    public void SetRoundIndex(int roundIndex)
    {
        _roundLabel.text = (roundIndex + 1).ToString();
    }

    public void HandleRoundOutcome(int playerIndex, bool success, float delta)
    {
        if (success)
            _successFeedback?.PlayFeedbacks();
        else
            _failureFeedback?.PlayFeedbacks();
    }
}
