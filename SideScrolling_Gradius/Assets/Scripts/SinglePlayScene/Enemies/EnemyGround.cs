using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGround : MonoBehaviour
{
    AudioSource audio;

    public AudioClip hitSound;
    public GameObject explosionEff;
    
    public Transform playerTrans;
    public Transform firePoint;

    [SerializeField] private int hp = 80;
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float firePower = 8.0f;

    [SerializeField] private bool isMove = false;

    public int HP { get; set; }
    private void Start()
    {
        audio = GetComponent<AudioSource>();
        HP = hp;

        StartCoroutine(Think());
        StartCoroutine(Fire());
    }

    private void Update()
    {
        if(isMove)
        {
            playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
            MoveMent();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PBullet"))
        {
            switch (collision.gameObject.name)
            {
                case "PBulletOne":
                    Damaged(10);
                    break;
                case "PBulletSpecial":
                    Damaged(20);
                    break;
                case "PBulletShell":
                    Damaged(100);
                    break;
            }
            PlaySound("hit");
        }
    }
    private void OnEnable()
    {
        if (PoolManager.Instance != null)
        {
            hp = 60;
            HP = hp;
        }
    }

    private void PlaySound(string name)
    {
        switch (name)
        {
            case "hit":
                audio.clip = hitSound;
                break;
        }
        audio.Play();
    }


    IEnumerator Think()
    {
        while(true)
        {
            isMove = true;
            yield return Cashing.YieldInstruction.WaitForSeconds(2.0f);
        }
    }
    IEnumerator Fire()
    {
        while (true)
        {
            for(int i = 0; i< 3; i++)
            {
                GameObject eg = PoolManager.Instance.MakeBullet("egbullet");
                eg.transform.position = firePoint.position;
                eg.SetActive(true);
                eg.GetComponent<Rigidbody2D>().AddForce(Vector2.up * firePower, ForceMode2D.Impulse);
                eg.name = "EBulletGround";
            }

            yield return Cashing.YieldInstruction.WaitForSeconds(4.0f);
        }
    }

    private void MoveMent()
    {
        Vector2 curPos = transform.position;

        if (transform.position.x < playerTrans.position.x)
        {
            transform.position = curPos + (Vector2.right * moveSpeed * Time.deltaTime);
        }
        else if (transform.position.x > playerTrans.position.x)
        {
            transform.position = curPos + (Vector2.left * moveSpeed * Time.deltaTime);
        }
        else
        {
            isMove = false;
        }
    }

    private void Dead()
    {
        SinglePlayManager.Instance.score += 10;

        GameObject effect = Instantiate(explosionEff, transform.position, transform.rotation);
        Destroy(effect, 0.2f);

        EnemySpawn.Instance.gCount--;
        this.gameObject.SetActive(false);
    }

    public void Damaged(int damage)
    {
        HP -= damage;

        if (HP <= 0)
        {
            Dead();
        }
    }
}
