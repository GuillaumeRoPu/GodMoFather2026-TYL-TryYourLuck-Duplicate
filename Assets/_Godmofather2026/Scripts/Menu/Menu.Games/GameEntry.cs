using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Entry", menuName = "Menu/Game Entry")]
public class GameEntry : ScriptableObject
{
    [BoxGroup("Identification")]
    [SerializeField] private string _gameId;

    [BoxGroup("Identification")]
    [SerializeField] private string _displayName;

    [BoxGroup("Presentation"), Required]
    [SerializeField] private Sprite _icon;

    [BoxGroup("Chargement")]
    [ValidateInput("IsSceneNameValid", "Cette scene doit etre ajoutee aux Build Settings")]
    [SerializeField] private string _sceneName;

    public string GameId => _gameId;
    public string DisplayName => _displayName;
    public Sprite Icon => _icon;
    public string SceneName => _sceneName;

    private bool IsSceneNameValid(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
            return false;

#if UNITY_EDITOR
        foreach (UnityEditor.EditorBuildSettingsScene buildSettingsScene in UnityEditor.EditorBuildSettings.scenes)
        {
            if (!buildSettingsScene.enabled)
                continue;

            string buildSettingsSceneName = System.IO.Path.GetFileNameWithoutExtension(buildSettingsScene.path);
            if (buildSettingsSceneName == sceneName)
                return true;
        }

        return false;
#else
        return true;
#endif
    }
}
