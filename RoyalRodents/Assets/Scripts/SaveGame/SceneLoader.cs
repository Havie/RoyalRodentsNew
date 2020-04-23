using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        // SceneManager.LoadScene(sceneName);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        StartCoroutine(LoadDelay(asyncLoad, false));
    }
    public void Save()
    {
        PlayerPrefs.SetInt("SceneSaved", SceneManager.GetActiveScene().buildIndex);
        SoundManager.Instance.PlayClick();
    }
    public void Load()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt(("SceneSaved")));
        SoundManager.Instance.PlayClick();
    }
    public void Quit()
    {
        SoundManager.Instance.PlayClick();
        Application.Quit();
    }
    public void LoadGame()
    {
        //Load the Scene in the BackGround
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        StartCoroutine(LoadDelay(asyncLoad, true));
    }
    IEnumerator LoadDelay(AsyncOperation async, bool LoadSave)
    {
        GameManager.Instance.StartScene();

        //The Setup for a Load Screen oO?
        while (!async.isDone)
            yield return null;

        GameManager.Instance.SceneStarted(true);
        //exists in the next Scene hence the wait
        if(LoadSave)
            sSaveManager.Instance.Load();
    }
}
