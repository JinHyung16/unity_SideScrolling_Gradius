using Nakama;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Nakama.TinyJson;

public class MultiEnemyChaser : MonoBehaviour
{
    private Rigidbody2D rigid2D;
    private Transform thisTrans;

    private Transform targetTrans;

    public float moveSpeed = 0.0f;

    //about sync
    public float stateFrequency = 0.05f;
    private float stateSyncTimer = 0.0f;

    public float LerpTime = 0.05f;

    private float lerpTimer = 0.0f;
    private Vector2 lerpFromPosition;
    private Vector2 lerpToPosition;
    private bool lerpPosition;

    private void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        thisTrans = rigid2D.GetComponent<Transform>();

        moveSpeed = 3.0f;
        stateFrequency = 0.01f;
        stateSyncTimer = 0.0f;

        //match시 현재 state에 대한 정보 넘기기 위해선 이런식의 연결이 필요하다.
        HughServer.GetInstance.Socket.ReceivedMatchState += EnqueueOnReceivedMatchState;
    }

    private void FixedUpdate()
    {
        Movement();
    }
    private void LateUpdate()
    {
        SyncInputEnemyPosition();
        SyncEnemyMovement();
    }

    // 양 측의 적이 생성된 위치 값 전달해주기
    private void SyncInputEnemyPosition()
    {
        if (stateSyncTimer <= 0)
        {
            GameManager.GetInstance.SendMatchState(OpCodes.EnemyPosition,
                MatchDataJson.EnemyPosition(rigid2D.velocity, thisTrans.position));

            stateSyncTimer = stateFrequency;
        }

        stateSyncTimer -= Time.deltaTime;
    }

    private void Movement()
    {
        if (GameManager.GetInstance.IsSpawnLocal && GameManager.GetInstance.IsSpawnRemote
            && !SceneController.GetInstance.IsSinglePlayScene())
        {
            Vector2 direction = Vector2.left;
            rigid2D.velocity = direction * moveSpeed;
        }
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
            thisTrans.position = lerpToPosition;
            lerpPosition = false;
        }

        thisTrans.position = Vector2.Lerp(lerpFromPosition, lerpToPosition, lerpTimer / LerpTime);
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
                UpdatePositionAndVelocity(matchState.State);
                break;
            default:
                break;
        }
    }

    private IDictionary<string, string> GetStateAsDictionary(byte[] state)
    {
        return Encoding.UTF8.GetString(state).FromJson<Dictionary<string, string>>();
    }


    private void UpdatePositionAndVelocity(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);

        rigid2D.velocity = new Vector2(float.Parse(stateDictionary["velocity.x"]),
            float.Parse(stateDictionary["velocity.y"]));

        var position = new Vector2(
            float.Parse(stateDictionary["position.x"]),
            float.Parse(stateDictionary["position.y"]));

        lerpFromPosition = thisTrans.position;
        lerpToPosition = position;
        lerpTimer = 0;
        lerpPosition = true;
    }
}
