using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed class PoolManager : MonoBehaviour
{
    #region SingleTon
    private static PoolManager instance;

    public static PoolManager Instance
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

            BindingObject();
            Setting();
            Pooling();
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    #endregion
    // item
    public GameObject powerItem;
    public GameObject shellItem;

    private GameObject[] powers;
    private GameObject[] shells;

    // enemy
    public GameObject eChaser;
    public GameObject eBoomber;
    public GameObject eUFO;
    public GameObject eGround;
    public GameObject eBoss;

    private GameObject[] chasers;
    private GameObject[] boombers;
    private GameObject[] ufos;
    private GameObject[] grounds;
    private GameObject boss;

    // bullet
    public GameObject pBullet;
    public GameObject pBulletSp;
    public GameObject pShell;
    public GameObject egBullet; // enemy ground bullet
    public GameObject euBullet; // enemy ufo bullet
    public GameObject ebBullet; // enemy boss bullet
    public GameObject ebShell;

    private GameObject[] pbullets;
    private GameObject[] pbulletsSp;
    private GameObject[] pshells;
    private GameObject[] egbullets;
    private GameObject[] eubullets;
    private GameObject[] ebbullets;
    private GameObject[] ebshells;

    private GameObject[] items;
    private GameObject[] enemys;
    private GameObject[] bullets;

    private void BindingObject()
    {
        if (powerItem == null)
        {
            powerItem = Resources.Load("PowerItem") as GameObject;
        }
        if (shellItem == null)
        {
            shellItem = Resources.Load("ShellItem") as GameObject;
        }
        if (eChaser == null)
        {
            eChaser = Resources.Load("EnemyChaser") as GameObject;
        }
        if (eBoomber == null)
        {
            eBoomber = Resources.Load("EnemyBoomber") as GameObject;
        }
        if (eUFO == null)
        {
            eUFO = Resources.Load("EnemyUFO") as GameObject;
        }
        if (eGround == null)
        {
            eGround = Resources.Load("EnemyGround") as GameObject;
        }
        if (eBoss == null)
        {
            eBoss = Resources.Load("EnemyBoss") as GameObject;
        }
        if (pBullet == null)
        {
            pBullet = Resources.Load("PBulletOne") as GameObject;
        }
        if (pBulletSp == null)
        {
            pBulletSp = Resources.Load("PBulletSpecial") as GameObject;
        }
        if (pShell == null)
        {
            pShell = Resources.Load("PBulletShell") as GameObject;
        }
        if (egBullet == null)
        {
            egBullet = Resources.Load("EBulletGround") as GameObject;
        }
        if (euBullet == null)
        {
            euBullet = Resources.Load("EBulletUFO") as GameObject;
        }
        if (ebBullet == null)
        {
            ebBullet = Resources.Load("EBulletBoss") as GameObject;
        }
        if (ebShell == null)
        {
            ebShell = Resources.Load("EBossShell") as GameObject;
        }
    }
    private void Setting()
    {
        powers = new GameObject[5];
        shells = new GameObject[5];

        chasers = new GameObject[7];
        boombers = new GameObject[16];
        ufos = new GameObject[4];
        grounds = new GameObject[5];

        pbullets = new GameObject[100];
        pbulletsSp = new GameObject[50];
        pshells = new GameObject[10];
        egbullets = new GameObject[100];
        eubullets = new GameObject[200];
        ebbullets = new GameObject[500];
        ebshells = new GameObject[30];
    }

    private void Pooling()
    {
        for(int i = 0; i<powers.Length; i++)
        {
            powers[i] = Instantiate(powerItem);
            powers[i].name = "PowerItem";
            powers[i].SetActive(false);
        }
        for (int i = 0; i < shells.Length; i++)
        {
            shells[i] = Instantiate(shellItem);
            shells[i].name = "ShellItem";
            shells[i].SetActive(false);
        }
        for (int i = 0; i<chasers.Length; i++)
        {
            chasers[i] = Instantiate(eChaser);
            chasers[i].name = "EnemyChaser";
            chasers[i].SetActive(false);
        }
        for (int i = 0; i < boombers.Length; i++)
        {
            boombers[i] = Instantiate(eBoomber);
            boombers[i].name = "EnemyBoomber";
            boombers[i].SetActive(false);
        }
        for (int i = 0; i < ufos.Length; i++)
        {
            ufos[i] = Instantiate(eUFO);
            ufos[i].name = "EnemyUFO";
            ufos[i].SetActive(false);
        }
        for (int i = 0; i < grounds.Length; i++)
        {
            grounds[i] = Instantiate(eGround);
            grounds[i].name = "EnemyGround";
            grounds[i].SetActive(false);
        }

        boss = Instantiate(eBoss);
        boss.name = "EnemyBoss";
        boss.SetActive(false);

        for(int i = 0; i<pbullets.Length; i++)
        {
            pbullets[i] = Instantiate(pBullet);
            pbullets[i].name = "PBulletOne";
            pbullets[i].SetActive(false);
        }
        for(int i = 0; i<pbulletsSp.Length; i++)
        {
            pbulletsSp[i] = Instantiate(pBulletSp);
            pbulletsSp[i].name = "PBulletSpecial";
            pbulletsSp[i].SetActive(false);
        }
        for (int i = 0; i < pshells.Length; i++)
        {
            pshells[i] = Instantiate(pShell);
            pshells[i].name = "PBulletShell";
            pshells[i].SetActive(false);
        }
        for (int i = 0; i < egbullets.Length; i++)
        {
            egbullets[i] = Instantiate(egBullet);
            egbullets[i].name = "EBulletGround";
            egbullets[i].SetActive(false);
        }
        for (int i = 0; i < eubullets.Length; i++)
        {
            eubullets[i] = Instantiate(euBullet);
            eubullets[i].name = "EBulletUFO";
            eubullets[i].SetActive(false);
        }
        for (int i = 0; i < ebbullets.Length; i++)
        {
            ebbullets[i] = Instantiate(ebBullet);
            ebbullets[i].name = "EBulletBoss";
            ebbullets[i].SetActive(false);
        }
        for (int i = 0; i < ebshells.Length; i++)
        {
            ebshells[i] = Instantiate(ebShell);
            ebshells[i].name = "EBossShell";
            ebshells[i].SetActive(false);
        }
    }

    public GameObject MakeItem(string name)
    {
        switch(name)
        {
            case "power":
                items = powers;
                break;
            case "shell":
                items = shells;
                break;
        }

        for(int i = 0; i<items.Length; i++)
        {
            if(!items[i].activeSelf)
            {
                items[i].SetActive(true);
                return items[i];
            }
        }

        return null;
    }
    public GameObject MakeEnemy(string name)
    {
        switch(name)
        {
            case "chaser":
                enemys = chasers;
                break;
            case "boomber":
                enemys = boombers;
                break;
            case "ufo":
                enemys = ufos;
                break;
            case "ground":
                enemys = grounds;
                break;
        }

        for(int i = 0; i < enemys.Length; i++)
        {
            if (!enemys[i].activeSelf)
            {
                enemys[i].SetActive(true);
                return enemys[i];
            }
        }
        return null;
    }

    public GameObject MakeBoss()
    {
        return boss;
    }

    public GameObject MakeBullet(string name)
    {
        switch(name)
        {
            case "pbullet":
                bullets = pbullets;
                break;
            case "pbulletSp":
                bullets = pbulletsSp;
                break;
            case "pshell":
                bullets = pshells;
                break;
            case "egbullet":
                bullets = egbullets;
                break;
            case "eubullet":
                bullets = eubullets;
                break;
            case "ebbullet":
                bullets = ebbullets;
                break;
            case "ebshell":
                bullets = ebshells;
                break;
        }

        for(int i = 0; i<bullets.Length; i++)
        {
            if(!bullets[i].activeSelf)
            {
                bullets[i].SetActive(true);
                return bullets[i];
            }
        }
        return null;
    }
}
