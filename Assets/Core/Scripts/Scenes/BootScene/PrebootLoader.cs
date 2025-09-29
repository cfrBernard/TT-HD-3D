using UnityEngine;
using System.Collections;

public class PrebootLoader : MonoBehaviour // PLACEHOLDER TODO : REFz3?
{
    IEnumerator Start()
    {
        yield return null;
        GameManager.Instance.SetGameState(GameState.Boot);
        SceneManager.Instance.LoadScene(SceneNames.Boot);
    }
}
