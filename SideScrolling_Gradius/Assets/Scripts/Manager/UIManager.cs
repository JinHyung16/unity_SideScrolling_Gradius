using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton
    private static UIManager instance;

    public static UIManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    private AudioSource audio;

    [Tooltip("Can Change the value used by range")]
    [Range(0, 120)][SerializeField] private float bossTime = 120.0f;

    public float curTime = 0.0f;
    private float startTime = 0.0f;

    [HideInInspector] public bool isOver = false;

    [HideInInspector] public int score = 0;
    [HideInInspector] public int pshellCount = 0;
    public int hp = 3;

    public GameObject SinglePlayCanvas;
    public GameObject ScoreCanvas;
    public GameObject GameStartCanvas;
    public GameObject ResultCanvas;

    public Text scoreText;
    public Text resultText;

    public Text resultScoreText;
    public Text pshellCountText;

    public Image[] hpImgs;

    public Button startBt;
    public Button exitBt;

    public AudioClip startSound;
    public AudioClip overSound;

    private void Start()
    {
        audio = GetComponent<AudioSource>();

        bossTime = Random.Range(60, 121);
        startTime = Time.time;

        scoreText.text = "SCORE " + score.ToString();

        startBt.onClick.AddListener(GameStart);
        exitBt.onClick.AddListener(ExitGame);

        CanvasActive("all", false);

        PlaySound("Start");

        // pause the singlePlayScene
        Time.timeScale = 0;
    }

    private void Update()
    {
        curTime += (Time.deltaTime - startTime);
        if (curTime > bossTime)
        {
            GameManager.GetInstance.isGroundStage = false;
            GameManager.GetInstance.isBossStage = true;

            EnemySpawn.GetInstance.BossSpawnController();
        }

        ScoreUpdate();

        if (isOver)
        {
            resultText.text = "Game Over";
        }
        else
        {
            resultText.text = "Game Clear";
        }
    }

    public void CanvasActive(string name, bool active)
    {
        switch (name)
        {
            case "single":
                SinglePlayCanvas.SetActive(active);
                break;
            case "score":
                ScoreCanvas.SetActive(active);
                break;
            case "gamestart":
                GameStartCanvas.SetActive(active);
                break;
            case "result":
                ResultCanvas.SetActive(active);
                break;
            default:
                SinglePlayCanvas.SetActive(active);
                ScoreCanvas.SetActive(active);
                GameStartCanvas.SetActive(active);
                ResultCanvas.SetActive(active);
                break;
        }
    }
    private void GameStart()
    {
        CanvasActive("gamestart", false);
        if (SceneController.GetInstace.IsSinglePlayScene())
        {
            CanvasActive("single", true);
        }
        CanvasActive("score", true);
        audio.Stop();
        Time.timeScale = 1;
    }

    private async void ExitGame()
    {
        SceneController.GetInstace.LoadScene("Main");

        CanvasActive("all", false);

        score = 0;
        GameManager.GetInstance.isGroundStage = false;
        GameManager.GetInstance.isBossStage = false;
        isOver = false;
        hp = 3;

        for (int i = 0; i < hpImgs.Length; i++)
        {
            hpImgs[i].color = new Color(1, 1, 1, 1);
        }

        Time.timeScale = 0;

        if (!SceneController.GetInstace.IsSinglePlayScene())
        {
            await GameManager.GetInstance.QuickMatch();
        }
    }

    private void GameOver()
    {
        isOver = true;
        GameManager.GetInstance.isBossStage = false;
        resultScoreText.text = "Score " + score.ToString();
        PlaySound("Over");
        CanvasActive("result", true);
        Time.timeScale = 0;
    }

    public void GameClear()
    {
        isOver = false;
        GameManager.GetInstance.isBossStage = false;
        GameManager.GetInstance.isGroundStage = false;

        CanvasActive("result", true);

        resultScoreText.text = "Score " + score.ToString();
        PlaySound("Over");

        Time.timeScale = 0;
    }

    private void PlaySound(string name)
    {
        switch (name)
        {
            case "Start":
                audio.clip = startSound;
                break;
            case "Over":
                audio.clip = overSound;
                break;
        }

        audio.Play();
    }

    public void HealthDown()
    {
        if (hp > 0)
        {
            hp--;
            hpImgs[hp].color = new Color(0, 0, 0, 0);
        }
        else
        {
            hpImgs[0].color = new Color(0, 0, 0, 0);
            GameOver();
        }
    }

    public void ScoreUpdate()
    {
        scoreText.text = "Score " + score.ToString();
        pshellCountText.text = "Boom " + pshellCount.ToString();
    }
}
