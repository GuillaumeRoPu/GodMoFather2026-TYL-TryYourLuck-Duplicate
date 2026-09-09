using MoreMountains.Feedbacks;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class ChronoDisplayView : MonoBehaviour
{
    [BoxGroup("References"), Required]
    [SerializeField] private TMP_Text _timeLabel;

    [BoxGroup("References")]
    [SerializeField] private GameObject _hiddenPlaceholder;

    [BoxGroup("Feedbacks")]
    [SerializeField] private MMF_Player _digitChangeFeedback;

    [BoxGroup("Feedbacks")]
    [SerializeField] private MMF_Player _buttonPressFeedback;

    public void SetElapsedTime(float elapsedTime)
    {
        _timeLabel.text = elapsedTime.ToString("F2");
        _digitChangeFeedback?.PlayFeedbacks();
    }

    public void SetVisibilityAlpha(float alpha)
    {
        Color color = _timeLabel.color;
        color.a = alpha;
        _timeLabel.color = color;

        bool isFullyHidden = alpha <= 0f;
        if (_hiddenPlaceholder != null)
            _hiddenPlaceholder.SetActive(isFullyHidden);
    }

    public void HandleChronoStopped(int playerIndex, float stoppedTime)
    {
        _buttonPressFeedback?.PlayFeedbacks();
    }
}
