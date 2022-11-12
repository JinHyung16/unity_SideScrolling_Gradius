using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaser : MonoBehaviour, IDamage
{
    AudioSource audio;

    public AudioClip hitSound;

    public GameObject explosionEff;
    public Transform playerTrans;

    [SerializeField] private int hp = 60;
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float rotateSpeed = 3.0f;
    public int HP { get; set; }

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        HP = hp;
    }

    private void Update()
    {
        if (SceneController.GetInstace.IsSinglePlayScene())
        {
            playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else
        {
            /*
            if (GameManager.GetInstance.IsSpawnLocal && GameManager.GetInstance.IsSpawnRemote)
            {
                playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
            }
            */
        }
        Movement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PBullet"))
        {
            switch(collision.gameObject.name)
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
        if (PoolManager.GetInstance != null)
        {
            hp = 40;
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
    private void Movement()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTrans.position, moveSpeed * Time.deltaTime);

        if (playerTrans != null)
        {
            Vector2 direction = new Vector2(transform.position.x - playerTrans.position.x, transform.position.y - playerTrans.position.y);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, rotateSpeed * Time.deltaTime);
            transform.rotation = rotation;
        }
    }

    private void Dead()
    {
        UIManager.GetInstance.score += 10;

        GameObject effect = Instantiate(explosionEff, transform.position, transform.rotation);
        Destroy(effect, 0.2f);

        EnemySpawn.GetInstance.cCount--;
        StateActiveSet();
    }

    private void StateActiveSet()
    {
        audio.Stop();
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
