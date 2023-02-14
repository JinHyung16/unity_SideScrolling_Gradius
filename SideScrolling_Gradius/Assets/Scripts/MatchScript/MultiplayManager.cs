using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughUtility;
using UnityEngine.UI;
using System.Text;

public class MultiplayManager : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    private int multiScore = 0;
    private int remoteScore = 0;

    //about observer
    [SerializeField] MultiEnemyChaser multiEnemyChaser;

    private void Start()
    {
        multiScore = 0;
        scoreText.text = multiScore.ToString();
    }


    public void UpdateScore(int score)
    {
        multiScore += score;
        scoreText.text = multiScore.ToString();

        ScoreSyncToServer();
    }

    public void UpdateScoreInServer(int score)
    {
        remoteScore = score;
        scoreText.text = remoteScore.ToString();
    }

    public async void QuickMatch()
    {
        await MatchManager.GetInstance.QuickMatch();
        SceneController.GetInstance.LoadScene("Main");
    }

    private async void ScoreSyncToServer()
    {
        await MatchManager.GetInstance.SendMatchStateAsync(OpCodes.Score, MatchDataJson.MultiScoreUpdate(multiScore));
    }
}
