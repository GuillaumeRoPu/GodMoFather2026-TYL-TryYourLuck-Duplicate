using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReelItemView : MonoBehaviour
{
    [BoxGroup("References"), Required]
    [SerializeField] private Image _gameIconImage;

    [BoxGroup("References"), Required]
    [SerializeField] private TMP_Text _gameNameLabel;

    public void SetData(GameEntry gameEntry)
    {
        _gameIconImage.sprite = gameEntry.Icon;
        _gameNameLabel.text = gameEntry.DisplayName;
    }
}
