using System.Collections;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private StringData sceneName;
    [SerializeField] private EncounterType sceneType;

    [SerializeField] private GameObject sceneBase;
    [SerializeField] private EncounterLoadEvent onEncounterLoadEvent;
    [SerializeField] private EncounterNameEvent onEncounterStartEvent;
    private ScreenFade screenFade;

    private bool isActive = true;

    private void Awake()
    {
        screenFade = FindAnyObjectByType<ScreenFade>();
        if (screenFade == null)
        {
            Debug.LogError("ScreenFade component not found in the scene.");
            return;
        }
    }

    private void OnEnable()
    {
        onEncounterLoadEvent.Subscribe(LoadScene);
    }

    private void OnDisable()
    {
        // Unsubscribe from the event to prevent memory leaks
        onEncounterLoadEvent.Unsubscribe(LoadScene);
    }

    private void LoadScene(string scene, string encounter)
    {
        if (scene == sceneName)
        {
            sceneBase.SetActive(true);
            isActive = true;
            StartCoroutine(LoadSceneCoroutine(encounter));
        }
        else if (isActive)
        {
            sceneBase.SetActive(false);
            isActive = false;
        }
    }

    private IEnumerator LoadSceneCoroutine(string encounter)
    {
        screenFade.FadeOut();
        onEncounterStartEvent.RaiseEvent(sceneType, encounter);

        yield return new WaitForSeconds(1f);

        screenFade.FadeIn();
    }
}
