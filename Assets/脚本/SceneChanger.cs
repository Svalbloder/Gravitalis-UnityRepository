using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 用于加载指定名称的场景
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 用于加载指定索引的场景
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // 在场景之间进行异步加载（例如加载下一场景时显示加载动画）
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {
            // 你可以在这里显示进度条等加载效果
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            // Debug.Log("加载进度: " + progress); // 打印加载进度
            yield return null;
        }
    }
}