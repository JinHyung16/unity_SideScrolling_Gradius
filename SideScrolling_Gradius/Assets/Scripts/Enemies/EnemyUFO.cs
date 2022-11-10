using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUFO : MonoBehaviour, IDamage
{
    SpriteRenderer sprite;
    AudioSource audio;

    public AudioClip hitSound;

    public GameObject explosionEff;
    public int numOfbulletCount;             // Number of projectiles to shoot.
    public float firePower;               // Speed of the projectile.

    private Vector2 startPoint;                 // Starting position of the bullet.
    private const float radius = 1.0f;

    [SerializeField] private int hp = 200;
    [SerializeField] private float moveSpeed = 0.8f;

    public int HP { get; set; }

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
        startPoint = transform.position;

        StartCoroutine(Fire(numOfbulletCount));
        HP = hp;

    }

    private void Update()
    {
        startPoint = this.transform.position;
        Movement();
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
            hp = 200;
            HP = hp;
        }
    }

    // Spawns x number of projectiles.
    IEnumerator Fire(int _fireCount)
    {
        while (true)
        {
            float angleStep = 360f / _fireCount;
            float angle = 0f;

            for (int i = 0; i <= _fireCount - 1; i++)
            {
                // Direction calculations.
                float fireDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
                float fireDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

                // Create vectors.
                Vector2 fireDirVector = new Vector2(fireDirXPosition, fireDirYPosition);
                Vector2 bulletMoveDirection = (fireDirVector - startPoint).normalized * firePower;

                // Create game objects.
                GameObject ub = PoolManager.GetInstance.MakeBullet("eubullet");
                ub.transform.position = new Vector2(startPoint.x, startPoint.y);
                ub.SetActive(true);
                ub.GetComponent<Rigidbody2D>().velocity = new Vector2(bulletMoveDirection.x, bulletMoveDirection.y);

                angle += angleStep;
            }
            yield return Cashing.YieldInstruction.WaitForSeconds(8.0f);
        }
    }

    private void PlaySound(string name)
    {
        switch(name)
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
        transform.position = curPos + (Vector2.left * moveSpeed * Time.deltaTime);
    }
    private void Dead()
    {
        UIManager.GetInstance.score += 50;

        sprite.color = new Color(1, 1, 1, 1f);

        GameObject effect = Instantiate(explosionEff, transform.position, transform.rotation);
        Destroy(effect, 0.2f);

        EnemySpawn.GetInstance.uCount--;
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

        HitOnEffect();

        if (HP <= 0)
        {
            Dead();
        }
    }

    private void HitOnEffect()
    {
        sprite.color = new Color(1, 1, 1, 0.5f);
        Invoke("HitOffEffect", 0.5f);
    }

    private void HitOffEffect()
    {
        sprite.color = new Color(1, 1, 1, 1);
    }
}
