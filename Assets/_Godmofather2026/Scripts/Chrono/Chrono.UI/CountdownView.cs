using MoreMountains.Feedbacks;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class CountdownView : MonoBehaviour
{
    [BoxGroup("References"), Required]
    [SerializeField] private TMP_Text _countdownLabel;

    [BoxGroup("Feedbacks")]
    [SerializeField] private MMF_Player _pulseFeedback;

    public void SetCountdownValue(int remainingSeconds)
    {
        _countdownLabel.text = remainingSeconds.ToString();
        _pulseFeedback?.PlayFeedbacks();
    }

    public void OnCountdownFinished()
    {
        _countdownLabel.text = string.Empty;
    }
}
