using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void Save()
    {
        PlayerPrefs.SetInt("SceneSaved", SceneManager.GetActiveScene().buildIndex);
    }
    public void Load()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt(("SceneSaved")));
    }

}
