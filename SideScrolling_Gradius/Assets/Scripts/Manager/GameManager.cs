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

    [Tooltip("Play Mode Canvase 및 하위 UI")]
    public GameObject playModePanel;
    public Button singlePlayBt;
    public Button multiPlayBt;

    //Match Data
    private IMatch currentMatch;
    private IUserPresence localUser;
    private string ticket;

    private GameObject localPlayer;
    private IDictionary<string, GameObject> players;

    public GameObject NetworkLocalPlayerPrefab;
    public GameObject NetworkRemotePlayerPrefab;

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

    private void MultiPlayMode()
    {
        SceneController.GetInstace.LoadScene("MultiPlay");
    }

    private async Task MatchStart(int min = 2)
    {
        var matchMakingTicket = await HughServer.GetInstace.Socket.AddMatchmakerAsync("*", min, 3);
        ticket = matchMakingTicket.Ticket;
    }

    private void SpawnPlayer(string matchId, IUserPresence user, int spawnIndex = -1)
    {
    }

    private async void OnRecivedMatchMakerMatched(IMatchmakerMatched matchmakerMatched)
    {
        // localuser 캐싱
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
        // 각 유저 참여시 스폰해주기
        foreach (var user in matchPresenceEvent.Joins)
        {
            SpawnPlayer(matchPresenceEvent.MatchId, user);
        }

        // 각 유저가 떠날 때 삭제해주기
        foreach (var user in matchPresenceEvent.Leaves)
        {
            Debug.Log("Leave User Session Id : " + user.SessionId);

            if (players.ContainsKey(user.SessionId))
            {
                Destroy(players[user.SessionId]);
                players.Remove(user.SessionId);
            }
        }
    }

    // 함수 내에서 await 사용 안해서 뜨는 노란 줄
    private async Task OnReceivedMatchState(IMatchState matchState)
    {
        // local 유저의 session id 가져오기
        var userSessionId = matchState.UserPresence.SessionId;

        // match state의 길이가 있다면 dictionary에 decode해주기
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        // OpCode에 따라 Match 상태 변경
        switch (matchState.OpCode)
        {
            case OpCodes.Died:
                var playerToDestroy = players[userSessionId];
                Destroy(playerToDestroy, 0.5f);
                players.Remove(userSessionId);
                if (players.Count == 0)
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
