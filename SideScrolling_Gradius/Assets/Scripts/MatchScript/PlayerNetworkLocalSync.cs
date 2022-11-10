using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkLocalSync : MonoBehaviour
{
    private InputController playerInputController;

    private Rigidbody2D rigidbody2D;
    private Transform playerTransform;

    public float StateSyncTimer = 0.1f;
    private float stateSyncTimer = 0.0f;

    private void Start()
    {
        playerInputController = GetComponent<InputController>();

        rigidbody2D = GetComponentInChildren<Rigidbody2D>();
        playerTransform = rigidbody2D.GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        if (stateSyncTimer <= 0)
        {

            GameManager.GetInstance.SendMatchState(OpCodes.Position,
                MatchDataJson.PositionAndVelocity(rigidbody2D.velocity, playerTransform.position));

            stateSyncTimer = StateSyncTimer;
        }

        stateSyncTimer -= Time.deltaTime;

        if (!playerInputController.inputChange)
        {
            return;
        }

        GameManager.GetInstance.SendMatchState(OpCodes.Input,
            MatchDataJson.Input(playerInputController.hInput, playerInputController.vInput, playerInputController.fireInput));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EBullet"))
        {
            GameManager.GetInstance.LocalPlayerDied(this.gameObject);
        }
    }
}
