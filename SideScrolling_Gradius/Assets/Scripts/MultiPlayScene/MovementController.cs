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

    // 0=�浹 ����, 1=��, -1=�Ʒ�, 2=����, -2=������ border�� �浹 �Ȱ� �ǹ�
    private int checkBorderPos = 0;

    private void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();

        moveSpeed = 0.5f;
        checkBorderPos = 0; //�⺻�� �浹 ���ѻ��·� �α�
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
