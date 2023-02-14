using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    SpriteRenderer sprite;
    Animator anim;
    AudioSource audio;

    public ParticleSystem upgradeParticle;
    public ParticleSystem moveParticle;

    Vector2 direction; // move dirceion
    public Transform firePointOne; // fire position one
    public Transform firePointTwo; // fire position twon
    public Transform firePointThree; // fire position three
    public Transform fireShellPoint; // fire shell position

    [Tooltip("ÃÑ ½î´Â Å° ¼³Á¤")]
    [SerializeField] private KeyCode fireKey = KeyCode.Space; // fire bullet key
    [SerializeField] private KeyCode shellKey = KeyCode.K; // fire shell bullet key
    [SerializeField] private KeyCode specialKey = KeyCode.L; // special fire bullet key
    [SerializeField] private int playerPower = 1;
    [SerializeField] private int shellCount = 3;
    [SerializeField] private int speicalCount; // Number of special to shoot.

    [SerializeField] private float moveSpeed = 0.0f; // move speed
    [SerializeField] private float firePower = 0.0f; // fire power

    private Vector2 startPoint; // Starting position of the bullet.
    private const float radius = 1.0f;

    private float startTime = 0.0f;
    [SerializeField] private float curTime = 0.0f;
    [SerializeField] private float specialTime = 0.0f;
    [SerializeField] private float delayTime = 0.0f;
    [SerializeField] private float specialDelay = 4.0f;

    private bool isTop = false;
    private bool isBottom = false;
    private bool isLeft = false;
    private bool isRight = false;

    public AudioClip shootClip;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();

        transform.position = new Vector2(-11, 0);
        startPoint = transform.position;

        delayTime = 1.2f;
        specialDelay = 4.0f;

        playerPower = 1;
        startTime = Time.time;
    }

    private void Update()
    {
        InputMove();
        ReloadTime();
        SpecialReloadTime();
        ShellUpdate();
        startPoint = this.transform.position;
    }

    private void FixedUpdate()
    {
        Movement();
        Fire();
        FireShell();
        SpecialFire(speicalCount);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("EBullet") || collision.CompareTag("Enemy"))
        {
            OnDamageBlink();
            SinglePlayManager.GetInstance.HealthDown();
        }
        if (collision.CompareTag("PowerItem"))
        {
            PowerUP();
        }
        if(collision.CompareTag("BoomItem"))
        {
            shellCount++;
        }
        if(collision.CompareTag("Border"))
        {
            switch(collision.gameObject.name)
            {
                case "Up":
                    isTop = true;
                    break;
                case "Down":
                    isBottom = true;
                    break;
                case "Left":
                    isLeft = true;
                    break;
                case "Right":
                    isRight = true;
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Border"))
        {
            switch (collision.gameObject.name)
            {
                case "Up":
                    isTop = false;
                    break;
                case "Down":
                    isBottom = false;
                    break;
                case "Left":
                    isLeft = false;
                    break;
                case "Right":
                    isRight = false;
                    break;
            }
        }
    }


    #region Private Method
    private void InputMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if ((isRight && h == 1) || (isLeft && h == -1))
        {
            h = 0;
        }

        float v = Input.GetAxisRaw("Vertical");
        if ((isTop && v == 1) || (isBottom && v == -1))
        {
            v = 0;
        }

        direction = new Vector2(h, v);
        anim.SetInteger("isVertical", (int)direction.y);

        if(h !=0 || v != 0)
        {
            moveParticle.Play();
        }
    }

    private void Movement()
    {
        Vector2 curPos = transform.position;
        Vector2 nextPos = direction * moveSpeed * Time.deltaTime;

        transform.position = curPos + nextPos;
    }
    private void ReloadTime()
    {
        curTime += (startTime - Time.deltaTime);
    }

    private void SpecialReloadTime()
    {
        specialTime += (startTime - Time.deltaTime);
    }
    private void Fire()
    {
        if (curTime > delayTime)
        {
            if(Input.GetKeyDown(fireKey))
            {
                if (0 < playerPower)
                {
                    GameObject b1 = NewPoolManager.GetInstance.GetPrefab(NewPoolManager.PoolableType.PBullet, "PBullet");
                    //GameObject b1 = PoolManager.GetInstance.MakeBullet("pbullet");
                    b1.transform.position = firePointOne.position;
                    b1.SetActive(true);
                    b1.GetComponent<Rigidbody2D>().AddForce(Vector2.right * firePower, ForceMode2D.Impulse);
                    if (1 < playerPower)
                    {
                        GameObject b2 = NewPoolManager.GetInstance.GetPrefab(NewPoolManager.PoolableType.PBullet, "PBullet");
                        //GameObject b2 = PoolManager.GetInstance.MakeBullet("pbullet");
                        b2.transform.position = firePointTwo.position;
                        b2.SetActive(true);
                        b2.GetComponent<Rigidbody2D>().AddForce(Vector2.right * firePower, ForceMode2D.Impulse);
                        if (2 < playerPower)
                        {
                            GameObject b3 = NewPoolManager.GetInstance.GetPrefab(NewPoolManager.PoolableType.PBullet, "PBullet");
                            //GameObject b3 = PoolManager.GetInstance.MakeBullet("pbullet");
                            b3.transform.position = firePointThree.position;
                            b3.SetActive(true);
                            b3.GetComponent<Rigidbody2D>().AddForce(Vector2.right * firePower, ForceMode2D.Impulse);
                        }
                    }
                }
                PlaySound("shoot");
                curTime = 0.0f;
            }
        }
    }
    private void FireShell()
    {
        if (Input.GetKeyDown(shellKey))
        {
            if (shellCount > 0)
            {
                GameObject s = NewPoolManager.GetInstance.GetPrefab(NewPoolManager.PoolableType.PShell, "PShell");
                //GameObject s = PoolManager.GetInstance.MakeBullet("pshell");
                s.transform.position = fireShellPoint.position;
                s.SetActive(true);
                s.GetComponent<Rigidbody2D>().AddForce(Vector2.down * firePower, ForceMode2D.Impulse);
                shellCount -= 1;
            }
            else
            {
                return;
            }
        }
    }
    private void SpecialFire(int _fireCount)
    {
        if (specialTime > specialDelay)
        {
            if (Input.GetKeyDown(specialKey))
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
                    GameObject bs = NewPoolManager.GetInstance.GetPrefab(NewPoolManager.PoolableType.PBullet, "PBulletSP");
                    //GameObject bs = PoolManager.GetInstance.MakeBullet("pbulletSp");
                    bs.transform.position = new Vector2(startPoint.x, startPoint.y);
                    bs.SetActive(true);
                    bs.GetComponent<Rigidbody2D>().velocity = new Vector2(bulletMoveDirection.x, bulletMoveDirection.y);
                    angle += angleStep;
                }
                specialTime = 0.0f;
            }
        }
    }

    private void PowerUP()
    {
        upgradeParticle.Play();
        playerPower++;
        if(playerPower > 3)
        {
            playerPower = 3;
        }
    }

    private void ShellUpdate()
    {
        if(shellCount <= 0 )
        {
            shellCount = 0;
        }

        SinglePlayManager.GetInstance.pshellCount = shellCount;
    }
    private void OnDamageBlink()
    {
        playerPower = 1;
        this.gameObject.layer = 9; // player damage layer
        sprite.color = new Color(1, 1, 1, 0.7f);

        Invoke("OffDamageBlink", 5.0f);
    }

    private void OffDamageBlink()
    {
        this.gameObject.layer = 6; // player layer
        sprite.color = new Color(1, 1, 1, 1);
    }

    private void PlaySound(string name)
    {
        switch(name)
        {
            case "shoot":
                audio.clip = shootClip;
                break;
        }
        audio.Play();
    }
    #endregion
}
