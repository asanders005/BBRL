using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private StringData targetSceneName;
    [SerializeField] private StringEvent onSceneLoad;

    public void LoadScene()
    {
        if (targetSceneName != null && !string.IsNullOrEmpty(targetSceneName.Value))
        {
            onSceneLoad.RaiseEvent(targetSceneName.Value);
        }
        else
        {
            Debug.LogWarning("Target scene name is not set or is empty.");
        }
    }
}
