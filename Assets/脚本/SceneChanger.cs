using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // ���ڼ���ָ�����Ƶĳ���
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ���ڼ���ָ�������ĳ���
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // �ڳ���֮������첽���أ����������һ����ʱ��ʾ���ض�����
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {
            // �������������ʾ�������ȼ���Ч��
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            // Debug.Log("���ؽ���: " + progress); // ��ӡ���ؽ���
            yield return null;
        }
    }
}