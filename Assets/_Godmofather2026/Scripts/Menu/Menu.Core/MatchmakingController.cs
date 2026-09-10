using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchmakingController : MonoBehaviour
{
    [BoxGroup("Registre"), Required]
    [SerializeField] private AvailableGamesRegistry _availableGamesRegistry;

    [BoxGroup("Animation")]
    [SerializeField] private float _minimumMatchmakingDuration = 5f;

    [BoxGroup("Channels"), Required]
    [SerializeField] private GameEntryGameEvent _onGameSelectedChannel;

    [BoxGroup("Channels"), Required]
    [SerializeField] private VoidGameEvent _onGameLoadingCompleteChannel;

    [BoxGroup("Channels"), Required]
    [SerializeField] private VoidGameEvent _onMatchmakingFinishedChannel;

    private readonly GameSelectionService _gameSelectionService = new GameSelectionService();
    private AsyncOperation _pendingSceneLoadOperation;

    public void HandlePlayButtonPressed()
    {
        GameEntry selectedGameEntry = _gameSelectionService.SelectRandomGame(_availableGamesRegistry.GetAllGameEntries());
        _onGameSelectedChannel.Raise(selectedGameEntry);

        StartCoroutine(LoadSelectedGame(selectedGameEntry));
    }

    private IEnumerator LoadSelectedGame(GameEntry selectedGameEntry)
    {
        float matchmakingStartTime = Time.time;

        AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(selectedGameEntry.SceneName, LoadSceneMode.Single);
        sceneLoadOperation.allowSceneActivation = false;

        while (sceneLoadOperation.progress < 0.9f)
            yield return null;

        float elapsedMatchmakingDuration = Time.time - matchmakingStartTime;
        float remainingMinimumDuration = _minimumMatchmakingDuration - elapsedMatchmakingDuration;
        if (remainingMinimumDuration > 0f)
            yield return new WaitForSeconds(remainingMinimumDuration);

        _pendingSceneLoadOperation = sceneLoadOperation;
        _onGameLoadingCompleteChannel.Raise();

        Debug.Log($"[Matchmaking] Jeu selectionne pret a etre lance : {selectedGameEntry.DisplayName}");
        _onMatchmakingFinishedChannel.Raise();
    }

    public void ActivateSelectedGameScene()
    {
        if (_pendingSceneLoadOperation == null)
            return;

        _pendingSceneLoadOperation.allowSceneActivation = true;
        _pendingSceneLoadOperation = null;
    }
}
