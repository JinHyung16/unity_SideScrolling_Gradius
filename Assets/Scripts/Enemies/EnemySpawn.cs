using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    #region SingleTon
    private static EnemySpawn instance;

    public static EnemySpawn Instance
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

    public Transform chaserSpawnPoint;
    public Transform boomberSpawnPoint;
    public Transform ufoSpawnPoint;
    public Transform groundSpawnPoint;
    public Transform bossSpawnPoint;

    [SerializeField] private float cMaxTime = 7.0f;
    [SerializeField] private float bMaxTime = 5.0f;
    [SerializeField] private float uMaxTime = 20.0f;
    [SerializeField] private float gMaxTime = 4.0f;

    private float cTime = 0.0f;
    private float bTime = 0.0f;
    private float uTime = 0.0f;
    private float gTime = 0.0f;
    private float startTime = 0.0f;

    private int chaserYAxis = 0;
    private int boomberYAxis = 0;
    private int ufoYAxis = 0;
    private int groudXAxis = 0;

    [SerializeField] private int chaserMax = 5;
    [SerializeField] private int boomberMax = 12;
    [SerializeField] private int ufoMax = 2;
    [SerializeField] private int groundMax = 3;

    public int cCount = 0;
    public int bCount = 0;
    public int uCount = 0;
    public int gCount = 0;

    private void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if(PoolManager.Instance != null)
        {
            if (GameManager.Instance.bossSpawn)
            {
                BossSpawn();
                GameManager.Instance.bossSpawn = false;
            }

            if (GameManager.Instance.isBossStage)
            {
                GameManager.Instance.curTime = 0.0f;
            }

            CountCheck();
            ReloadTime();

            if (cCount < chaserMax)
            {
                if (cTime > cMaxTime)
                {
                    ChaserSpawn();
                }
            }


            if (bCount < (boomberMax - 3))
            {
                if (bTime > bMaxTime)
                {
                    BoomberSpawn();
                }
            }


            if (uCount < ufoMax)
            {
                if (uTime > uMaxTime)
                {
                    UfoSpawn();
                }
            }

            if (gCount < groundMax)
            {
                if (gTime > gMaxTime)
                {
                    GroundSpawn();
                }
            }
        }
        else
        {
            return;
        }
    }

    private void CountCheck()
    {
        if(cCount <= 0)
        {
            cCount = 0;
        }
        if (bCount <= 0)
        {
            bCount = 0;
        }
        if (uCount <= 0)
        {
            uCount = 0;
        }
        if (gCount <= 0)
        {
            gCount = 0;
        }
    }
    private void ReloadTime()
    {
        cTime += (Time.deltaTime - startTime);
        bTime += (Time.deltaTime - startTime);
        uTime += (Time.deltaTime - startTime);
        gTime += (Time.deltaTime - startTime);
    }
    
    private void BossSpawn()
    {
        GameObject boss = PoolManager.Instance.MakeBoss();
        boss.transform.position = bossSpawnPoint.position;
        boss.SetActive(true);
    }
    private void ChaserSpawn()
    {
        if (!GameManager.Instance.isBossStage)
        {
            chaserYAxis = Random.Range(-7, 7);
            GameObject ec = PoolManager.Instance.MakeEnemy("chaser");
            ec.transform.position = new Vector2(chaserSpawnPoint.position.x, chaserYAxis);
            cCount++;
            cTime = 0.0f;
        }
        else
        {
            cTime = 0.0f;
            return;
        }
    }
    private void BoomberSpawn()
    {
        boomberYAxis = Random.Range(-4, 4);
        // 한번에 본인 기준 위 아래 총 3개 생성
        for (int i = -1; i < 2; i++)
        {
            GameObject eb = PoolManager.Instance.MakeEnemy("boomber");
            eb.transform.position = new Vector2(boomberSpawnPoint.position.x, boomberYAxis + i);
            eb.SetActive(true);
            bCount++;
        }
        bTime = 0.0f;
    }
    private void UfoSpawn()
    {
        if (!GameManager.Instance.isBossStage)
        {
            ufoYAxis = Random.Range(-4, 4);
            GameObject eu = PoolManager.Instance.MakeEnemy("ufo");
            eu.transform.position = new Vector2(ufoSpawnPoint.position.x, ufoYAxis);
            uCount++;
            uTime = 0.0f;
        }
        else
        {
            uTime = 0.0f;
            return;
        }
    }

    private void GroundSpawn()
    {
        if (GameManager.Instance.isGroundStage && !GameManager.Instance.isBossStage)
        {
            groudXAxis = Random.Range(-8, 5);
            GameObject eg = PoolManager.Instance.MakeEnemy("ground");
            eg.transform.position = new Vector2(groudXAxis, groundSpawnPoint.position.y);
            gCount++;
            gTime = 0.0f;
        }
        else
        {
            gTime = 0.0f;
            return;
        }
    }
}
