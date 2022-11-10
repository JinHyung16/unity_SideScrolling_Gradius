using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoomber : MonoBehaviour, IDamage
{
    SpriteRenderer sprite;
    AudioSource audio;

    public AudioClip hitSound;
    public ParticleSystem particle;
    public GameObject boomEffect;

    [SerializeField] private int hp = 50;
    [SerializeField] private float moveSpeed = 4.0f;

    public int HP { get; set; }

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
        HP = hp;

        StartCoroutine(ReadyBoom());
    }

    private void Update()
    {
        Movement();
    }

    IEnumerator ReadyBoom()
    {
        while(true)
        {
            sprite.color = new Color(1, 1, 1, 0.5f);
            yield return Cashing.YieldInstruction.WaitForSeconds(0.5f);
            sprite.color = new Color(1, 1, 1, 1);
            yield return Cashing.YieldInstruction.WaitForSeconds(0.5f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Dead();
        }

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

        if (collision.CompareTag("Border"))
        {
            if (collision.gameObject.name == "Left")
            {
                Dead();
            }
        }
    }
    private void OnEnable()
    {
        if (PoolManager.GetInstance != null)
        {
            hp = 50;
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
        Vector2 curPos = transform.position;
        transform.position = curPos + Vector2.left * moveSpeed * Time.deltaTime;
        particle.Play();
    }

    private void Dead()
    {
        UIManager.GetInstance.score += 5;

        GameObject effect = Instantiate(boomEffect, transform.position, transform.rotation);
        Destroy(effect, 0.2f);

        EnemySpawn.GetInstance.bCount--;
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
