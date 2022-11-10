using HughUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : LazySingleton<SceneController>
{
    public void LoadScene(string scene)
    {
        if (SceneManager.GetSceneByName(scene) == null)
        {
            return;
        }
        else
        {
            switch (scene)
            {
                case "Main":
                    SceneManager.LoadScene("Main");
                    break;
                case "SinglePlay":
                    SceneManager.LoadScene("SinglePlay");
                    break;
                case "MultiPlay":
                    SceneManager.LoadScene("MultiPlay");
                    break;
            }
        }
    }

    public bool IsSinglePlayScene()
    {
        if (SceneManager.GetActiveScene().name == "SinglePlay")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
