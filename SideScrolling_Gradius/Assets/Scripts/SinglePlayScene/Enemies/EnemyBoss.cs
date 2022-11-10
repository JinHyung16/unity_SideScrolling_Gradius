using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour, IDamage
{
    Animator anim;
    SpriteRenderer sprite;

    public enum MoveState
    {
        fmove,
        bmove,
        hmove,
    }

    public MoveState mstate;

    public GameObject explosion;

    public ParticleSystem middleStateParticle;
    public ParticleSystem finalStateParticle;
    public ParticleSystem explosionParticle;

    public Transform firePointOne;
    public Transform firePointTwo;
    public Transform firePointThree;
    public Transform firePointFour;
    public Transform firePointFive;
    public Transform playerTrans;

    [SerializeField] private int hp = 1000;
    [SerializeField] private float moveSpeed = 7.0f;
    [SerializeField] private float firePower = 12.0f;

    public int numOfbulletCount;
    private Vector2 startPoint;
    private float radius = 1.0f;


    private float moveThinkTime = 0.0f;
    [SerializeField] private float yMoveAxis = 0.0f;
    [SerializeField] private float xMoveAxis = 0.0f;


    public int HP { get; set; }

    private void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        HP = hp;

        startPoint = transform.position;

        StartCoroutine(AttackTime());
        StartCoroutine(MoveThink());
    }

    private void Update()
    {
        startPoint = this.transform.position;
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        StateChange();
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
                    Damaged(50);
                    break;
            }
        }
    }
    IEnumerator AttackTime()
    {
        while (true)
        {
            switch (mstate)
            {
                case MoveState.fmove:
                    Fire();
                    break;
                case MoveState.bmove:
                    BoomFire();
                    break;
                case MoveState.hmove:
                    for (int i = 0; i < 2; i++)
                    {
                        HellFire(numOfbulletCount);
                    }
                    break;
            }
            yield return Cashing.YieldInstruction.WaitForSeconds(3.0f);
        }
    }

    IEnumerator MoveThink()
    {
        while(true)
        {
            xMoveAxis = Random.Range(6.0f, 10.0f);
            yMoveAxis = Random.Range(-5.0f, 5.0f);
            moveThinkTime = Random.Range(0.5f, 2.0f);
            yield return Cashing.YieldInstruction.WaitForSeconds(moveThinkTime);
        }
    }

    private void StateChange()
    {
        if(0 < HP)
        {
            if(2000 < HP && HP <= 3000)
            {
                mstate = MoveState.fmove;
            }
            if(700 < HP && HP <= 2000)
            {
                middleStateParticle.Play();
                mstate = MoveState.bmove;
            }
            if(HP<=700)
            {
                finalStateParticle.Play();
                mstate = MoveState.hmove;
            }    
        }
    }

    private void Movement()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(xMoveAxis, yMoveAxis), moveSpeed * Time.deltaTime);

        /*
        if(mstate == MoveState.fmove || mstate == MoveState.bmove)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(xMoveAxis, yMoveAxis), moveSpeed * Time.deltaTime);
        }
        else if(mstate == MoveState.bmove)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(0, 0), moveSpeed * Time.deltaTime);
        }
        */
    }

    private void Fire()
    {
        GameObject bb1 = PoolManager.GetInstance.MakeBullet("ebbullet");
        bb1.transform.position = firePointTwo.position;
        bb1.SetActive(true);
        bb1.GetComponent<Rigidbody2D>().AddForce(Vector2.left * firePower, ForceMode2D.Impulse);

        GameObject bb2 = PoolManager.GetInstance.MakeBullet("ebbullet");
        bb2.transform.position = firePointThree.position;
        bb2.SetActive(true);
        bb2.GetComponent<Rigidbody2D>().AddForce(Vector2.left * firePower, ForceMode2D.Impulse);

        GameObject bb3 = PoolManager.GetInstance.MakeBullet("ebbullet");
        bb3.transform.position = firePointFour.position;
        bb3.SetActive(true);
        bb3.GetComponent<Rigidbody2D>().AddForce(Vector2.left * firePower, ForceMode2D.Impulse);
    }

    private void BoomFire()
    {
        Fire();

        GameObject bs1 = PoolManager.GetInstance.MakeBullet("ebshell");
        bs1.transform.position = firePointOne.position;
        bs1.SetActive(true);

        GameObject bs2 = PoolManager.GetInstance.MakeBullet("ebshell");
        bs2.transform.position = firePointTwo.position;
        bs2.SetActive(true);

        GameObject bs3 = PoolManager.GetInstance.MakeBullet("ebshell");
        bs3.transform.position = firePointThree.position;
        bs3.SetActive(true);

        GameObject bs4 = PoolManager.GetInstance.MakeBullet("ebshell");
        bs4.transform.position = firePointFour.position;
        bs4.SetActive(true);

        GameObject bs5 = PoolManager.GetInstance.MakeBullet("ebshell");
        bs5.transform.position = firePointFive.position;
        bs5.SetActive(true);
    }

    private void HellFire(int _fireCount)
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
            GameObject ub = PoolManager.GetInstance.MakeBullet("ebbullet");
            ub.transform.position = new Vector2(startPoint.x, startPoint.y);
            ub.SetActive(true);
            ub.GetComponent<Rigidbody2D>().velocity = new Vector2(bulletMoveDirection.x, bulletMoveDirection.y);

            angle += angleStep;
        }
    }

    private void Dead()
    {
        SinglePlayManager.GetInstance.score += 500;
        SinglePlayManager.GetInstance.isOver = false;

        anim.SetBool("isDead", true);
        explosionParticle.Play();

        GameObject effect = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(effect, 0.3f);

        Invoke("StateSetActive", 3.0f);
    }

    private void StateSetActive()
    {
        this.gameObject.SetActive(false);
        SinglePlayManager.GetInstance.GameClear();
    }

    public void Damaged(int damage)
    {
        HP -= damage;

        HitOnEff();

        if (HP <= 0)
        {
            Dead();
        }
    }

    private void HitOnEff()
    {
        sprite.color = new Color(1, 1, 1, 0.7f);
        Invoke("HitOffEff", 0.5f);
    }

    private void HitOffEff()
    {
        sprite.color = new Color(1, 1, 1, 1);
    }
}
