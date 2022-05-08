using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static string nextScene = SceneName._01_Title;

    public static string loadingSceneName = SceneName._99_Loading;
    [SerializeField]
    private Image progressBar;

    private void Start()
    {
        Fade.In(0.5f);

        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene(loadingSceneName);
    }

    float progress;

    public float waitTime = 1.0f;
    IEnumerator LoadScene()
    {
        yield return null;
        progressBar.fillAmount = 0f;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        yield return new WaitForSeconds(waitTime);

        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            if (op.progress >= 0.9f)
            {
                //progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

                //if (progressBar.fillAmount == 1.0f)
                //    op.allowSceneActivation = true;

                break;
            }
            else
            {
                //progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                //if (progressBar.fillAmount >= op.progress)
                //{
                //    timer = 0f;
                //}
            }
        }

        Fade.Out(0.5f, () => { op.allowSceneActivation = true; });
    }
}
