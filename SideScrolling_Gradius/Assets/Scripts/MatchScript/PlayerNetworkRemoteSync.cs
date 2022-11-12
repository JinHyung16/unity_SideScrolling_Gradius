using Nakama;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Nakama.TinyJson;

public class PlayerNetworkRemoteSync : MonoBehaviour
{
    [HideInInspector]
    public RemotePlayerNetworkData netWorkData;

    public MovementController movementController;
    public WeaponController weaponController;

    //interpolation to the player move speed -> 플레이어 움직임 동기화 자연스럽게
    public float LerpTime = 0.05f;

    public Rigidbody2D rigid2D;
    public Transform playerTransform;

    private float lerpTimer = 0.0f;
    private Vector2 lerpFromPosition;
    private Vector2 lerpToPosition;
    private bool lerpPosition;

    private void Start()
    {
        HughServer.GetInstace.Socket.ReceivedMatchState += EnqueueOnReceivedMatchState;

        movementController = GetComponentInChildren<MovementController>();
        weaponController = GetComponentInChildren<WeaponController>();

        rigid2D = GetComponentInChildren<Rigidbody2D>();
        playerTransform = rigid2D.GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        if (!lerpPosition)
        {
            return;
        }
        lerpTimer += Time.deltaTime;

        if (lerpTimer >= LerpTime)
        {
            playerTransform.position = lerpToPosition;
            lerpPosition = false;
        }

        playerTransform.position = Vector2.Lerp(lerpFromPosition, lerpToPosition, lerpTimer / LerpTime);
    }

    private void OnDestroy()
    {
        if (GameManager.GetInstance != null)
        {
            HughServer.GetInstace.Socket.ReceivedMatchState -= EnqueueOnReceivedMatchState;
        }
        //GameManager.GetInstance.IsSpawnRemote = false;
    }

    private void OnEnable()
    {
        //GameManager.GetInstance.IsSpawnRemote = true;
    }

    private void EnqueueOnReceivedMatchState(IMatchState matchState)
    {
        var mainThread = UnityMainThreadDispatcher.Instance();
        mainThread.Enqueue(() => OnReceivedMatchState(matchState));
    }

    private void OnReceivedMatchState(IMatchState matchState)
    {
        // If the incoming data is not related to this remote player, ignore it and return early.
        if (matchState.UserPresence.SessionId != netWorkData.User.SessionId)
        {
            return;
        }

        // Decide what to do based on the Operation Code of the incoming state data as defined in OpCodes.
        switch (matchState.OpCode)
        {
            case OpCodes.Position:
                UpdatePositionAndVelocity(matchState.State);
                break;
            case OpCodes.Input:
                SetInputFromState(matchState.State);
                break;
            case OpCodes.Died:
                movementController.Death(this.gameObject);
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

        lerpFromPosition = playerTransform.position;
        lerpToPosition = position;
        lerpTimer = 0;
        lerpPosition = true;
    }

    private void SetInputFromState(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);

        movementController.SetDirectionMovement(float.Parse(stateDictionary["horizontalInput"]),
            float.Parse(stateDictionary["verticalInput"]));

        if (bool.Parse(stateDictionary["fireInput"]))
        {
            weaponController.AttackFire();
        }
    }
}
