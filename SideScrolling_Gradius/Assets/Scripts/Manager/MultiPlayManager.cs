using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiPlayManager : MonoBehaviour
{

    public Button exitBt;

    private void Start()
    {
        exitBt.onClick.AddListener(ExitMultiPlay);
    }

    private async void ExitMultiPlay()
    {
        await GameManager.GetInstace.QuickMatch();
        SceneController.GetInstace.LoadScene("Main");
    }
}
