using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

sealed class SinglePlayManager : MonoBehaviour
{/*
    #region SingleTon
    private static SinglePlayManager instance;

    public static SinglePlayManager GetInstance
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
        else
        {
            Destroy(this.gameObject);
        }

    }
    #endregion

    private AudioSource audio;

    [Tooltip("Can Change the value used by range")]
    [Range(0, 120)][SerializeField] private float bossTime = 120.0f;

    public float curTime = 0.0f;
    private float startTime = 0.0f;

    public bool isOver = false;

    public int score = 0;
    public int hp = 3;
    public int pshellCount = 0;

    public GameObject initPanel;
    public GameObject resultPanel;

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

        initPanel.SetActive(true);
        resultPanel.SetActive(false);

        scoreText.text = "SCORE " + score.ToString();
        startBt.onClick.AddListener(GameStart);
        exitBt.onClick.AddListener(ExitGame);

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

            EnemySpawn.GetInstance.BossSpawnController(true);
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

    private void GameStart()
    {
        initPanel.SetActive(false);

        audio.Stop();
        Time.timeScale = 1;
    }

    private void ExitGame()
    {
        initPanel.SetActive(false);
        resultPanel.SetActive(false);

        score = 0;
        GameManager.GetInstance.isGroundStage = false;
        GameManager.GetInstance.isBossStage = false;
        isOver = false;
        hp = 3;

        for(int i = 0; i<hpImgs.Length; i++)
        {
            hpImgs[i].color = new Color(1, 1, 1, 1);
        }

        Time.timeScale = 0;

        SceneController.GetInstace.LoadScene("Main");
    }

    private void GameOver()
    {
        isOver = true;
        GameManager.GetInstance.isBossStage = false;

        resultPanel.SetActive(true);
        resultScoreText.text = "Score " + score.ToString();

        PlaySound("Over");
        Time.timeScale = 0;
    }

    public void GameClear()
    {
        isOver = false;
        GameManager.GetInstance.isBossStage = false;
        GameManager.GetInstance.isGroundStage = false;

        resultPanel.SetActive(true);
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
    */
}
