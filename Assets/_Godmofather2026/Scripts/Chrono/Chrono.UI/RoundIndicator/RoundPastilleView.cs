using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class RoundPastilleView : MonoBehaviour
{
    [BoxGroup("References"), Required]
    [SerializeField] private Image _pastilleImage;

    private bool _hasBeenColored;

    public void SetUnplayedColor(Color unplayedColor)
    {
        _hasBeenColored = false;
        _pastilleImage.color = unplayedColor;
    }

    public void ApplyResultColor(Color resultColor)
    {
        if (_hasBeenColored)
            return;

        _hasBeenColored = true;
        _pastilleImage.color = resultColor;
    }
}
