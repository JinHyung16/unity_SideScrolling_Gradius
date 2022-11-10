using Nakama;
using Nakama.TinyJson;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    #region SingleTon
    private static GameManager instance;
    public static GameManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    //map ���� flag
    [HideInInspector]
    public bool isBossStage = false;

    [HideInInspector]
    public bool isGroundStage = false;

    [Tooltip("Play Mode Canvase �� ���� UI")]
    public GameObject PlayModePanel;
    public Button singlePlayBt;
    public Button multiPlayBt;

    //Match Data
    private IMatch currentMatch;
    private IUserPresence localUser;
    private string ticket;

    private GameObject localPlayer;
    private IDictionary<string, GameObject> playerDictionary;

    //���� �÷��̾�, ����Ʈ �÷��̾� ĳ��
    public GameObject NetworkLocalPlayerPrefab;
    public GameObject NetworkRemotePlayerPrefab;

    //�÷��̾� ���� ��ġ �ޱ�
    public GameObject spawnPoint;

    private async void Start()
    {
        if (this.gameObject != null)
        {
            //cashing
            singlePlayBt.onClick.AddListener(SinglePlayMode);
            multiPlayBt.onClick.AddListener(MultiPlayMode);

            //panel setting
            PlayModePanel.SetActive(true);

            //about nakama server
            await HughServer.GetInstace.ConnecToServer();

            var mainThread = new UnityMainThreadDispatcher(); 
            HughServer.GetInstace.Socket.ReceivedMatchmakerMatched += m => mainThread.Enqueue(() => OnRecivedMatchMakerMatched(m));
            HughServer.GetInstace.Socket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnReceivedMatchPresence(m));
            HughServer.GetInstace.Socket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGroundStage = true;
        }
    }
    private void SinglePlayMode()
    {
        PlayModePanel.SetActive(false);
        
        SceneController.GetInstace.LoadScene("SinglePlay");
        EnemySpawn.GetInstance.EnemyCoroutineController(true);
    }

    private async void MultiPlayMode()
    {
        PlayModePanel.SetActive(false);
        await MatchStart();

        SceneController.GetInstace.LoadScene("MultiPlay");
        EnemySpawn.GetInstance.EnemyCoroutineController(true);
    }

    private async Task MatchStart(int min = 2)
    {
        var matchMakingTicket = await HughServer.GetInstace.Socket.AddMatchmakerAsync("*", min, 3);
        ticket = matchMakingTicket.Ticket;
#if UNITY_EDITOR
        Debug.LogFormat("<color=green><b>[Find Match]</b> Ticket : {0} </color>", ticket);
#endif
    }

    private void SpawnPlayer(string matchId, IUserPresence user, int spawnIndex = -1)
    {
        if (playerDictionary.ContainsKey(user.SessionId))
        {
            return;
        }

        var spawns = this.gameObject.transform;
        if (spawnIndex == -1)
        {
            spawns = spawnPoint.transform.GetChild(Random.Range(0, spawnPoint.transform.childCount));
        }
        else
        {
            spawns = spawnPoint.transform.GetChild(spawnIndex);
        }

        var isLocalPlayer = user.SessionId == localUser.SessionId;
        var playerPrefab = isLocalPlayer ? NetworkLocalPlayerPrefab : NetworkRemotePlayerPrefab;

        var player = Instantiate(playerPrefab, spawns.position, Quaternion.identity);

        if (!isLocalPlayer)
        {
            player.GetComponent<PlayerNetworkRemoteSync>().netWorkData = new RemotePlayerNetworkData
            {
                MatchId = matchId,
                User = user
            };
        }

        playerDictionary.Add(user.SessionId, player);

        if (isLocalPlayer) { localPlayer = player; }
    }

    public async void LocalPlayerDied(GameObject player)
    {
        await SendMatchStateAsync(OpCodes.Died, MatchDataJson.Died(player.transform.position));

        playerDictionary.Remove(localUser.SessionId);
        Destroy(player, 0.5f);
    }
    public async Task QuickMatch()
    {
        await HughServer.GetInstace.Socket.LeaveMatchAsync(currentMatch);

        currentMatch = null;
        localUser = null;

        foreach (var player in playerDictionary.Values)
        {
            Destroy(player);
        }

        playerDictionary.Clear();

#if UNITY_EDITOR
        Debug.Log("<color=green><br> Quick Match </br></color>");
#endif
    }

    public async Task SendMatchStateAsync(long opCode, string state)
    {
        await HughServer.GetInstace.Socket.SendMatchStateAsync(currentMatch.Id, opCode, state);
    }

    public void SendMatchState(long opCode, string state)
    {
        HughServer.GetInstace.Socket.SendMatchStateAsync(currentMatch.Id, opCode, state);
    }

    private async void OnRecivedMatchMakerMatched(IMatchmakerMatched matchmakerMatched)
    {
        // localuser ĳ��
        localUser = matchmakerMatched.Self.Presence;
        var match = await HughServer.GetInstace.Socket.JoinMatchAsync(matchmakerMatched);

#if UNITY_EDITOR
        Debug.Log("Our Session Id: " + match.Self.SessionId);
#endif

        foreach (var user in match.Presences)
        {
            Debug.Log("Connected User Session Id: " + user.SessionId);
            SpawnPlayer(match.Id, user);
        }

        currentMatch = match;
    }
    private void OnReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        // �� ���� ������ �������ֱ�
        foreach (var user in matchPresenceEvent.Joins)
        {
            SpawnPlayer(matchPresenceEvent.MatchId, user);
        }

        // �� ������ ���� �� �������ֱ�
        foreach (var user in matchPresenceEvent.Leaves)
        {
            Debug.Log("Leave User Session Id : " + user.SessionId);

            if (playerDictionary.ContainsKey(user.SessionId))
            {
                Destroy(playerDictionary[user.SessionId]);
                playerDictionary.Remove(user.SessionId);
            }
        }
    }

    // �Լ� ������ await ��� ���ؼ� �ߴ� ��� ��
    private async Task OnReceivedMatchState(IMatchState matchState)
    {
        // local ������ session id ��������
        var userSessionId = matchState.UserPresence.SessionId;

        // match state�� ���̰� �ִٸ� dictionary�� decode���ֱ�
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        // OpCode�� ���� Match ���� ����
        switch (matchState.OpCode)
        {
            case OpCodes.Died:
                var playerToDestroy = playerDictionary[userSessionId];
                Destroy(playerToDestroy, 0.5f);
                playerDictionary.Remove(userSessionId);
                if (playerDictionary.Count == 0)
                {
                    //GameOver();
                }
                break;
            case OpCodes.Respawn:
                SpawnPlayer(currentMatch.Id, matchState.UserPresence, int.Parse(state["spawnIndex"]));
                break;
            default:
                break;
        }

    }
}
