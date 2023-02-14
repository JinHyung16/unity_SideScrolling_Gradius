using HughUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : LazySingleton<SceneController>
{
    public void LoadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName) == null)
        {
            return;
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
