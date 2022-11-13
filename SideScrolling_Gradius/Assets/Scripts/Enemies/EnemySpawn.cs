using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    #region SingleTon
    private static EnemySpawn instance;

    public static EnemySpawn GetInstance
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

            BindingIEnumerator();
        }
    }
    #endregion

    public Transform chaserSpawnPoint;
    public Transform boomberSpawnPoint;
    public Transform ufoSpawnPoint;
    public Transform groundSpawnPoint;
    public Transform bossSpawnPoint;

    [SerializeField] private float cMaxTime = 7.0f;
    [SerializeField] private float bMaxTime = 5.0f;
    [SerializeField] private float uMaxTime = 20.0f;
    [SerializeField] private float gMaxTime = 4.0f;


    private int chaserYAxis = 0;
    private int boomberYAxis = 0;
    private int ufoYAxis = 0;
    private int groudXAxis = 0;

    [SerializeField] private int chaserMax = 5;
    [SerializeField] private int boomberMax = 12;
    [SerializeField] private int ufoMax = 2;
    [SerializeField] private int groundMax = 3;

    [HideInInspector] public int cCount = 0;
    [HideInInspector] public int bCount = 0;
    [HideInInspector] public int uCount = 0;
    [HideInInspector] public int gCount = 0;

    // 코루틴 Start, Stop하기 위해 관리하는 방법
    IEnumerator bossIEnum;
    IEnumerator chaserIEnum;
    IEnumerator boomberIEnum;
    IEnumerator ufoIEnum;
    IEnumerator groundIEnum;

    IEnumerator mChaserIEnum; //multi play enemy chaser spawning

    private void BindingIEnumerator()
    {
        bossIEnum = BossSpawn();
        chaserIEnum = ChaserSpawn();
        boomberIEnum = BoomberSpawn();
        ufoIEnum = UfoSpawn();
        groundIEnum = GroundSpawn();

        mChaserIEnum = MultiChaserSpawn();
    }

    public void BossSpawnController()
    {
        if (GameManager.GetInstance.isBossStage)
        {
            StartCoroutine(bossIEnum);
        }
        StopCoroutine(bossIEnum);
    }

    public void EnemyCoroutineController(bool active)
    {
        if (active == true)
        {
            StartCoroutine(chaserIEnum);
            StartCoroutine(boomberIEnum);
            StartCoroutine(ufoIEnum);
            if (SceneController.GetInstance.IsSinglePlayScene())
            {
                StartCoroutine(groundIEnum);
            }
        }
        else
        {
            StopCoroutine(chaserIEnum);
            StopCoroutine(boomberIEnum);
            StopCoroutine(ufoIEnum);
            StopCoroutine(groundIEnum);
        }
    }

    public void MultiEnemyStartCoroutine()
    {
        StartCoroutine(mChaserIEnum);
    }
    private IEnumerator BossSpawn()
    {
        GameObject boss = PoolManager.GetInstance.MakeBoss();
        boss.transform.position = bossSpawnPoint.position;
        boss.SetActive(true);

        yield return null;
    }

    private IEnumerator ChaserSpawn()
    {
        while (true)
        {
            if (cCount < chaserMax)
            {
                chaserYAxis = Random.Range(-7, 7);
                GameObject ec = PoolManager.GetInstance.MakeEnemy("chaser");
                ec.transform.position = new Vector2(chaserSpawnPoint.position.x, chaserYAxis);
                cCount++;
            }

            yield return Cashing.YieldInstruction.WaitForSeconds(cMaxTime);
        }
    }
    private IEnumerator BoomberSpawn()
    {
        while (true)
        {
            if (bCount < boomberMax)
            {
                boomberYAxis = Random.Range(-4, 4);
                // 한번에 본인 기준 위 아래 총 3개 생성
                for (int i = -1; i < 2; i++)
                {
                    GameObject eb = PoolManager.GetInstance.MakeEnemy("boomber");
                    eb.transform.position = new Vector2(boomberSpawnPoint.position.x, boomberYAxis + i);
                    eb.SetActive(true);
                    bCount++;
                }
            }
            yield return Cashing.YieldInstruction.WaitForSeconds(bMaxTime);
        }
    }
    private IEnumerator UfoSpawn()
    {
        while (true)
        {
            if (uCount < ufoMax)
            {
                ufoYAxis = Random.Range(-4, 4);
                GameObject eu = PoolManager.GetInstance.MakeEnemy("ufo");
                eu.transform.position = new Vector2(ufoSpawnPoint.position.x, ufoYAxis);
                uCount++;
            }
            yield return Cashing.YieldInstruction.WaitForSeconds(uMaxTime);
        }
    }

    private IEnumerator GroundSpawn()
    {
        while (true)
        {
            if (GameManager.GetInstance.isGroundStage && gCount < groundMax 
                && !GameManager.GetInstance.isBossStage)
            {
                groudXAxis = Random.Range(-8, 5);
                GameObject eg = PoolManager.GetInstance.MakeEnemy("ground");
                eg.transform.position = new Vector2(groudXAxis, groundSpawnPoint.position.y);
                gCount++;
            }

            yield return Cashing.YieldInstruction.WaitForSeconds(gMaxTime);
        }
    }

    private IEnumerator MultiChaserSpawn()
    {
        while (true)
        {
            chaserYAxis = Random.Range(-7, 7);
            GameObject mec = PoolManager.GetInstance.MakeEnemy("multiChaser");
            mec.transform.position = new Vector2(chaserSpawnPoint.position.x, chaserYAxis);
            yield return Cashing.YieldInstruction.WaitForSeconds(4.0f);
        }
    }
}
