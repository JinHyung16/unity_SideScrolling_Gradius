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
    public static GameManager GetInstace
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

    [Tooltip("Play Mode Canvase �� ���� UI")]
    public GameObject playModePanel;
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

            //about nakama server
            await HughServer.GetInstace.ConnecToServer();

            var mainThread = new UnityMainThreadDispatcher(); 
            HughServer.GetInstace.Socket.ReceivedMatchmakerMatched += m => mainThread.Enqueue(() => OnRecivedMatchMakerMatched(m));
            HughServer.GetInstace.Socket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnReceivedMatchPresence(m));
            HughServer.GetInstace.Socket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));
        }
    }

    private void SinglePlayMode()
    {
        SceneController.GetInstace.LoadScene("SinglePlay");
    }

    private async void MultiPlayMode()
    {
        SceneController.GetInstace.LoadScene("MultiPlay");

        await MatchStart();
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

        var isLocalPlayer = user.SessionId == localUser.SessionId;
        var playerPrefab = isLocalPlayer ? NetworkLocalPlayerPrefab : NetworkRemotePlayerPrefab;

        var player = Instantiate(playerPrefab, transform.position, Quaternion.identity);

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
