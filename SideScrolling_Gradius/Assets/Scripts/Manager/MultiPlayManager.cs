using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MultiPlayManager : MonoBehaviour
{

    [Tooltip("Cashing the InGame Canvase")]
    public GameObject InGameCanvase;
    [Tooltip("Cashing the Result Canvase")]
    public GameObject ResultCanvas;

    [Tooltip("Exit Button")]
    public Button exitBt;

    [Tooltip("InGame Score Text")]
    public Text inScoreTxt;

    [Tooltip("Result Score Text")]
    public Text scoreTxt;

    private void Start()
    {
        if (InGameCanvase == null)
        {
            InGameCanvase = Resources.Load("UICanvas/InGame Canvas") as GameObject;
        }
        if (InGameCanvase == null)
        {
            ResultCanvas = Resources.Load("UICanvas/Result Canvas") as GameObject;
        }

        //binding text
        if (inScoreTxt == null)
        {
            inScoreTxt = InGameCanvase.GetComponentsInChildren<Text>().First<Text>();
        }
        if (inScoreTxt == null)
        {
            scoreTxt = ResultCanvas.GetComponentsInChildren<Text>().First<Text>();
        }

        //binding button
        if (exitBt == null)
        {
            exitBt = ResultCanvas.GetComponentsInChildren<Button>().First<Button>();
        }

        InGameCanvase.SetActive(true);
        ResultCanvas.SetActive(false);
        exitBt.onClick.AddListener(ExitMultiPlay);
    }

    private async void ExitMultiPlay()
    {
        await GameManager.GetInstance.QuickMatch();
        SceneController.GetInstace.LoadScene("Main");
    }
}
