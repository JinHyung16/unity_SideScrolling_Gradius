using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkLocalSync : MonoBehaviour
{
    public InputController playerInputController;

    public Rigidbody2D rigidbody2D;
    private Transform playerTransform;

    public float stateFrequency = 0.1f;
    private float stateSyncTimer = 0.0f;

    private void Start()
    {
        playerInputController = GetComponent<InputController>();

        rigidbody2D = GetComponentInChildren<Rigidbody2D>();
        playerTransform = rigidbody2D.GetComponent<Transform>();

        stateFrequency = 0.05f;
    }

    private void LateUpdate()
    {
        if (stateSyncTimer <= 0)
        {
            MatchManager.GetInstance.SendMatchState(OpCodes.Position,
                MatchDataJson.Position(rigidbody2D.velocity, playerTransform.position));

            stateSyncTimer = stateFrequency;
        }

        stateSyncTimer -= Time.deltaTime;

        if (!playerInputController.inputChange)
        {
            return;
        }

        MatchManager.GetInstance.SendMatchState(OpCodes.Input,
            MatchDataJson.Input(playerInputController.hInput, playerInputController.vInput, playerInputController.fireInput));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EBullet"))
        {
            MatchManager.GetInstance.LocalPlayerDied(this.gameObject);
        }
    }

    
    private void OnEnable()
    {
        GameManager.GetInstance.IsSpawnLocal = true;
    }

    private void OnDestroy()
    {
        GameManager.GetInstance.IsSpawnLocal = false;
    }
    
}
