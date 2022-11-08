using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Rigidbody2D rigid2D;
    private Vector2 direction = Vector2.zero;

    [SerializeField] private float moveSpeed = 0.0f;

    private float horizontalMovement;
    private float verticalMovement;

    // 0=충돌 없음, 1=위, -1=아래, 2=왼쪽, -2=오른쪽 border와 충돌 된거 의미
    private int checkBorderPos = 0;

    private void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();

        moveSpeed = 0.5f;
        checkBorderPos = 0; //기본은 충돌 안한상태로 두기
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Border"))
        {
            switch (collision.gameObject.name)
            {
                case "Up":
                    checkBorderPos = 1;
                    break;
                case "Down":
                    checkBorderPos = -1;
                    break;
                case "Left":
                    checkBorderPos = 2;
                    break;
                case "Right":
                    checkBorderPos = -2;
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
                    checkBorderPos = 0;
                    break;
                case "Down":
                    checkBorderPos = 0;
                    break;
                case "Left":
                    checkBorderPos = 0;
                    break;
                case "Right":
                    checkBorderPos = 0;
                    break;
            }
        }
    }

    private void Movement()
    {
        direction = new Vector2(horizontalMovement, verticalMovement);
        rigid2D.velocity = direction * moveSpeed;
    }

    #region Public Function
    public void SetDirectionMovement(float horizontal, float vertical)
    {
        this.horizontalMovement = horizontal;
        this.verticalMovement = vertical;
    }

    public int GetCheckBorder()
    {
        return checkBorderPos;
    }

    public void DeathAnimation()
    {

    }
    #endregion
}
