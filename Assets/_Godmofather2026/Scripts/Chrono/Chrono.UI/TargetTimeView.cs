using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class TargetTimeView : MonoBehaviour
{
    [BoxGroup("References"), Required]
    [SerializeField] private TMP_Text _targetTimeLabel;

    public void SetTargetTime(float targetTime)
    {
        _targetTimeLabel.text = targetTime.ToString("F2") + "s";
    }
}
