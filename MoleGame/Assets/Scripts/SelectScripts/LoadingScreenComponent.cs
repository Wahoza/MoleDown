using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenComponent : MonoBehaviour
{
    [SerializeField] Image loadingBar;
    [SerializeField] Vector3 startHeight;
    [SerializeField] float dropDownDuration;
    [SerializeField] ShaderVariantCollection svc;

    private bool isIntroComplete;

    private bool isCompleted = false;
    private void Start()
    {
        transform.localPosition = startHeight;
    }
    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    private IEnumerator LoadSceneAsync(int sceneId)
    {
        isCompleted = false;
        isIntroComplete = false;
        transform.localPosition = startHeight;
        transform.DOLocalMoveY(0, dropDownDuration).SetEase(Ease.OutQuad).OnComplete(() => isIntroComplete = true);


        yield return new WaitUntil(() => isIntroComplete);

        StartCoroutine(StartLoading(sceneId));

        yield return null;
    }

    private IEnumerator StartLoading(int sceneId)
    {
        Debug.Log("LoadingScene");

        if (svc != null)
        {
            Debug.Log("Warming up shaders...");
            svc.WarmUp();
            Debug.Log("Shader warmup completed.");
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        while (!operation.isDone && operation.progress < 0.9f)
        {
            float progressVal = Mathf.Clamp01(operation.progress / 0.9f);

            Debug.Log(operation.progress);

            loadingBar.fillAmount = progressVal;
            yield return null;
        }

        yield return new WaitUntil(() => svc.isWarmedUp);
        // Fully loaded, now wait for input

        isCompleted = true;
        loadingBar.fillAmount = 1f;

        operation.allowSceneActivation = true;

        while (!operation.isDone)
            yield return null;

        yield return new WaitUntil(() => LevelLoadManager.Instance != null);

        LevelLoadManager.Instance.LoadLevel();

        Scene newScene = SceneManager.GetSceneByBuildIndex(sceneId);
        Scene oldScene = SceneManager.GetActiveScene();

        // Make the new scene active
        SceneManager.SetActiveScene(newScene);

        // Now unload the old scene
        SceneManager.UnloadSceneAsync(oldScene);

        LevelLoadManager.Instance.CompleteLoad();

        yield return null;

    }

}
