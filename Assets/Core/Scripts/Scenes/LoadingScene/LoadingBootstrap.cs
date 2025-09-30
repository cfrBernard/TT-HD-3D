using UnityEngine;
using System.Collections;

public class LoadingBootstrap : MonoBehaviour // PLACEHOLDER TODO : REFz3?
{
    private void Start()
    {
        StartCoroutine(LoadTargetScene());
    }

    private IEnumerator LoadTargetScene()
    {
        yield return new WaitForSeconds(1f);
        string target = SceneManager.Instance.GetTargetSceneToLoad();
        yield return SceneManager.Instance.LoadSceneAsync(target, true);

        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("LoadingScene");
    }
}
