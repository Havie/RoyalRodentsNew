using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        GameManager.Instance.StartScene();
    }
    public void Save()
    {
        PlayerPrefs.SetInt("SceneSaved", SceneManager.GetActiveScene().buildIndex);
    }
    public void Load()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt(("SceneSaved")));
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void LoadGame()
    {
        //Load the Scene in the BackGround
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        StartCoroutine(LoadDelay(asyncLoad));
    }
    IEnumerator LoadDelay(AsyncOperation async)
    {
        GameManager.Instance.StartScene();

        //The Setup for a Load Screen oO?
        while (!async.isDone)
            yield return null;

        GameManager.Instance.SceneStarted(true);
        //exists in the next Scene hence the wait
        sSaveManager.Instance.Load();
    }
}
