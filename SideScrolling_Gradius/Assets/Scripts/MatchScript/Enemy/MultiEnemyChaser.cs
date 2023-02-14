using Nakama;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Nakama.TinyJson;
using HughUtility;

public class MultiEnemyChaser : MonoBehaviour
{
    private MultiplayManager multiplayManager;

    private Rigidbody2D rigid2D;
    private Transform transform;

    private Transform targetTrans;

    private int enemyHP = 100;

    public float moveSpeed = 0.0f;

    //about sync
    public float stateFrequency = 0.05f;
    private float stateSyncTimer = 0.0f;

    public float LerpTime = 0.05f;
    private float lerpTimer = 0.0f;
    private Vector2 lerpFromPosition;
    private Vector2 lerpToPosition;
    private bool lerpPosition;

    private void OnEnable()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        transform = rigid2D.GetComponent<Transform>();

        multiplayManager = GameObject.FindGameObjectWithTag("MultiplayManager").GetComponent<MultiplayManager>();

        moveSpeed = 3.0f;
        stateFrequency = 0.01f;
        stateSyncTimer = 0.0f;

        enemyHP = 100;

        HughServer.GetInstance.Socket.ReceivedMatchState += EnqueueOnReceivedMatchState;
    }

    private void OnDisable()
    {
        HughServer.GetInstance.Socket.ReceivedMatchState -= EnqueueOnReceivedMatchState;
    }

    private void FixedUpdate()
    {
        Movement();
        SyncEnemyMovement();
    }
    private void LateUpdate()
    {
        //SyncInputEnemyPosition();
        //SyncEnemyMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PBullet"))
        {
            OnDamaged(10);
        }
    }
    #region private Function
    private void Movement()
    {
        if (GameManager.GetInstance.IsSpawnLocal && GameManager.GetInstance.IsSpawnRemote
            && !GameManager.GetInstance.IsSinglePlayScene)
        {
            rigid2D.velocity = Vector2.left * moveSpeed;

            MatchManager.GetInstance.SendMatchState(OpCodes.EnemyPosition, MatchDataJson.EnemyPosition(rigid2D.velocity, transform.position));
        }
    }

    // 양 측의 적이 생성된 위치 값 전달해주기
    private void SyncInputEnemyPosition()
    {
        if (stateSyncTimer <= 0)
        {
            MatchManager.GetInstance.SendMatchState(OpCodes.EnemyPosition,
                MatchDataJson.EnemyPosition(rigid2D.velocity, transform.position));

            stateSyncTimer = stateFrequency;
        }

        stateSyncTimer -= Time.deltaTime;
    }

    //실제 양 측 화면에 그려주는 역할
    private void SyncEnemyMovement()
    {
        if (!lerpPosition)
        {
            return;
        }
        lerpTimer += Time.deltaTime;

        if (lerpTimer >= LerpTime)
        {
            transform.position = lerpToPosition;
            lerpPosition = false;
        }

        transform.position = Vector2.Lerp(lerpFromPosition, lerpToPosition, lerpTimer / LerpTime);
    }
    private void EnqueueOnReceivedMatchState(IMatchState matchState)
    {
        var mainThread = UnityMainThreadDispatcher.Instance();
        mainThread.Enqueue(() => OnReceivedMatchState(matchState));
    }

    private void OnReceivedMatchState(IMatchState matchState)
    {
        // Decide what to do based on the Operation Code of the incoming state data as defined in OpCodes.
        switch (matchState.OpCode)
        {
            case OpCodes.EnemyPosition:
                SetPositionAndVelocity(matchState.State);
                break;
            case OpCodes.EnemyDie:
                SetEnemyDiePos(matchState.State);
                break;
            default:
                break;
        }
    }

    private IDictionary<string, string> GetStateAsDictionary(byte[] state)
    {
        return Encoding.UTF8.GetString(state).FromJson<Dictionary<string, string>>();
    }


    private void SetPositionAndVelocity(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);

        rigid2D.velocity = new Vector2(float.Parse(stateDictionary["enemy_velocity_x"]),
            float.Parse(stateDictionary["enemy_velocity_y"]));

        var position = new Vector3(
            float.Parse(stateDictionary["enemy_position_x"]),
            float.Parse(stateDictionary["enemy_position_y"]),
            0);

        lerpFromPosition = transform.position;
        lerpToPosition = position;
        lerpTimer = 0;
        lerpPosition = true;
    }

    private void SetEnemyDiePos(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);

        var position = new Vector2(
            float.Parse(stateDictionary["enemy_die_pos_x"]),
            float.Parse(stateDictionary["enemy_die_pos_y"]));

        multiplayManager.UpdateScore(10);
        MatchManager.GetInstance.curEnemyCount -= 1;
        NewPoolManager.GetInstance.DespawnObject(NewPoolManager.PoolableType.MultiChaser, this.gameObject);
    }
    #endregion

    #region public Function

    private void OnDamaged(int damage)
    {
        enemyHP -= damage;
        if (enemyHP <= 0)
        {
            MatchManager.GetInstance.SendMatchState(OpCodes.EnemyDie, MatchDataJson.EnemyDiePos(transform.position));
        }
    }
    #endregion
}
