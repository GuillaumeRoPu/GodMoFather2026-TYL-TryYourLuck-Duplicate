using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakingView : MonoBehaviour
{
    [BoxGroup("Recherche"), Required]
    [SerializeField] private RectTransform _searchingRingTransform;

    [BoxGroup("Recherche"), Required]
    [SerializeField] private TMP_Text _searchingLabel;

    [BoxGroup("Reveal"), Required]
    [SerializeField] private GameObject _revealPanel;

    [BoxGroup("Reveal"), Required]
    [SerializeField] private Image _revealedGameIconImage;

    [BoxGroup("Reveal"), Required]
    [SerializeField] private TMP_Text _revealedGameNameLabel;

    [BoxGroup("Animation")]
    [SerializeField] private float _ringRotationSpeedInDegreesPerSecond = 180f;

    private bool _isSearching;
    private GameEntry _lastSelectedGameEntry;

    public void HandlePlayButtonPressed()
    {
        _isSearching = true;
        _lastSelectedGameEntry = null;

        _revealPanel.SetActive(false);
        _searchingLabel.gameObject.SetActive(true);
        _searchingRingTransform.gameObject.SetActive(true);
    }

    public void HandleGameSelected(GameEntry selectedGameEntry)
    {
        _lastSelectedGameEntry = selectedGameEntry;
    }

    public void HandleGameLoadingComplete()
    {
        _isSearching = false;

        _searchingLabel.gameObject.SetActive(false);
        _searchingRingTransform.gameObject.SetActive(false);

        RevealSelectedGame();
    }

    private void RevealSelectedGame()
    {
        if (_lastSelectedGameEntry == null)
            return;

        _revealedGameIconImage.sprite = _lastSelectedGameEntry.Icon;
        _revealedGameNameLabel.text = _lastSelectedGameEntry.DisplayName;
        _revealPanel.SetActive(true);
    }

    private void Update()
    {
        if (!_isSearching)
            return;

        _searchingRingTransform.Rotate(0f, 0f, -_ringRotationSpeedInDegreesPerSecond * Time.deltaTime);
    }
}
