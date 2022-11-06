using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class MultiPlayManager : MonoBehaviour
{

    public Button exitBt;

    private async void Start()
    {
        exitBt.onClick.AddListener(ExitMultiPlay);
    }

    private void ExitMultiPlay()
    {
        SceneController.GetInstace.LoadScene("Main");
    }
}
