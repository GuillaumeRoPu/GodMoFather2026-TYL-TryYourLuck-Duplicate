using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchmakingReelController : MonoBehaviour
{
    [BoxGroup("Registre"), Required]
    [SerializeField] private AvailableGamesRegistry _availableGamesRegistry;

    [BoxGroup("References"), Required]
    [SerializeField] private SlotMachineReelView _slotMachineReelView;

    [BoxGroup("Lancement"), MinValue(0)]
    [SerializeField] private float _delayBeforeSceneActivation = 2f;

    [BoxGroup("Channels"), Required]
    [SerializeField] private GameEntryGameEvent _onGameSelectedChannel;

    [BoxGroup("Channels"), Required]
    [SerializeField] private GameEntryGameEvent _onReadyToRevealChannel;

    [BoxGroup("Channels"), Required]
    [SerializeField] private VoidGameEvent _onMatchmakingFinishedChannel;

    private readonly GameSelectionService _gameSelectionService = new GameSelectionService();
    private AsyncOperation _pendingSceneLoadOperation;

    private void OnEnable()
    {
        _slotMachineReelView.OnStopAnimationComplete += HandleReelStopAnimationComplete;
    }

    private void OnDisable()
    {
        _slotMachineReelView.OnStopAnimationComplete -= HandleReelStopAnimationComplete;
    }

    public void HandlePlayButtonPressed()
    {
        _slotMachineReelView.StartSpin();

        GameEntry selectedGameEntry = _gameSelectionService.SelectRandomGame(_availableGamesRegistry.GetAllGameEntries());
        _onGameSelectedChannel.Raise(selectedGameEntry);

        StartCoroutine(LoadSelectedGame(selectedGameEntry));
    }

    private IEnumerator LoadSelectedGame(GameEntry selectedGameEntry)
    {
        AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(selectedGameEntry.SceneName, LoadSceneMode.Single);
        sceneLoadOperation.allowSceneActivation = false;

        while (sceneLoadOperation.progress < 0.9f)
            yield return null;

        _pendingSceneLoadOperation = sceneLoadOperation;
        _onReadyToRevealChannel.Raise(selectedGameEntry);
    }

    public void HandleReadyToReveal(GameEntry selectedGameEntry)
    {
        _slotMachineReelView.StopOnGame(selectedGameEntry);
    }

    private void HandleReelStopAnimationComplete()
    {
        Debug.Log("[Matchmaking] Reel arrete, matchmaking termine");
        _onMatchmakingFinishedChannel.Raise();

        StartCoroutine(ActivateSelectedGameSceneAfterDelay());
    }

    private IEnumerator ActivateSelectedGameSceneAfterDelay()
    {
        yield return new WaitForSeconds(_delayBeforeSceneActivation);
        ActivateSelectedGameScene();
    }

    public void ActivateSelectedGameScene()
    {
        if (_pendingSceneLoadOperation == null)
            return;

        _pendingSceneLoadOperation.allowSceneActivation = true;
        _pendingSceneLoadOperation = null;
    }
}
