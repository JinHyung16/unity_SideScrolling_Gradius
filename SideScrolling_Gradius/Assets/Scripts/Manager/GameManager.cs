using UnityEngine;
using UnityEngine.UI;
using HughUtility;

public class GameManager : Singleton<GameManager>
{
    //map ���� flag
    [HideInInspector]
    public bool isBossStage = false;

    [HideInInspector]
    public bool isGroundStage = false;

    [HideInInspector]
    public bool IsSinglePlayScene = false;

    [Tooltip("Play Mode Canvase �� ���� UI")]
    public GameObject PlayModePanel;
    public Button singlePlayBt;
    public Button multiPlayBt;

    [HideInInspector] public bool IsSpawnLocal = false;
    [HideInInspector] public bool IsSpawnRemote = false;

    private void Start()
    {
        if (this.gameObject != null)
        {
            //cashing
            singlePlayBt.onClick.AddListener(SinglePlayMode);
            multiPlayBt.onClick.AddListener(MultiPlayMode);

            //panel setting
            PlayModePanel.SetActive(true);
        }
    }

    /// <summary>
    /// GameManager���� GroundStage �����ߴ��� üũ���ش�.
    /// </summary>
    /// <param name="collision"> �浹ü�� �Ű������� �޴´� </param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGroundStage = true;
        }
    }

    private void SinglePlayMode()
    {
        PlayModePanel.SetActive(false);
        IsSinglePlayScene = true;

        SceneController.GetInstance.LoadScene("SinglePlay");
        SinglePlayManager.GetInstance.CanvasActive("gamestart", true);
        EnemySpawn.GetInstance.EnemyCoroutineController(true);
    }

    private void MultiPlayMode()
    {
        PlayModePanel.SetActive(false);
        IsSinglePlayScene = false;

        SceneController.GetInstance.LoadScene("MultiPlay");
    }
}
