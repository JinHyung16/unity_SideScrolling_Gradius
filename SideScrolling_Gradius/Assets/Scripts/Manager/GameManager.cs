using UnityEngine;
using UnityEngine.UI;
using HughUtility;

public class GameManager : Singleton<GameManager>
{
    //map 관련 flag
    [HideInInspector]
    public bool isBossStage = false;

    [HideInInspector]
    public bool isGroundStage = false;

    [HideInInspector]
    public bool IsSinglePlayScene = false;

    [Tooltip("Play Mode Canvase 및 하위 UI")]
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
    /// GameManager에서 GroundStage 진입했는지 체크해준다.
    /// </summary>
    /// <param name="collision"> 충돌체를 매개변수로 받는다 </param>
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
