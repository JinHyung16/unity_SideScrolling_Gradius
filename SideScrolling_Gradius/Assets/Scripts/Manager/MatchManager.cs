using Nakama;
using Nakama.TinyJson;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Collections;

public class MatchManager : MonoBehaviour
{
    #region Singleton _ Only use in multiplay scene
    private static MatchManager instance;
    public static MatchManager GetInstance
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
        }
    }
    #endregion
    [Tooltip("MathScene의 UI들 관련")]
    public GameObject StartCanvas;
    [SerializeField] private Button matchBt;

    //Multiplay Enemy Spawn시키기
    private int chaserYAxis;
    private IEnumerator MultiEnemySpawnIEnum;
    [HideInInspector] public int curEnemyCount = 0;

    public MultiplayManager multiplayManager;

    //Match Data
    private IMatch currentMatch;
    private IUserPresence localUser;
    private string ticket;

    private GameObject localPlayer;
    private IDictionary<string, GameObject> playerDictionary;

    //로컬 플레이어, 리모트 플레이어 캐싱
    public GameObject NetworkLocalPlayerPrefab;
    public GameObject NetworkRemotePlayerPrefab;

    //플레이어 스폰 위치 받기
    public GameObject spawnPoint;

    private void OnDestroy()
    {
        StopCoroutine(MultiEnemySpawnIEnum);
    }

    private async void Start()
    {
        StartCanvas.SetActive(true);
        MultiEnemySpawnIEnum = SpawnMultiChaserCoroutine();

        //about nakama server
        playerDictionary = new Dictionary<string, GameObject>();
        var mainThread = UnityMainThreadDispatcher.Instance();

        await HughServer.GetInstance.ConnecToServer();

        HughServer.GetInstance.Socket.ReceivedMatchmakerMatched += m => mainThread.Enqueue(() => OnRecivedMatchMakerMatched(m));
        HughServer.GetInstance.Socket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnReceivedMatchPresence(m));
        HughServer.GetInstance.Socket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));
    }

    #region Button UI에 연결할 함수
    public async void FindMatch()
    {
        StartCanvas.SetActive(false);
        await MatchStart();
    }
    #endregion

    #region Multiplay Enemy Spawn 함수

    private void SpanwEnemyInMultiplay()
    {
        StartCoroutine(MultiEnemySpawnIEnum);
    }

    private IEnumerator SpawnMultiChaserCoroutine()
    {
        while (true)
        {
            if (curEnemyCount <= 5)
            {
                chaserYAxis = Random.Range(-7, 7);
                GameObject mec = NewPoolManager.GetInstance.GetPrefab(NewPoolManager.PoolableType.MultiChaser, "MultiEnemyChaser");
                mec.transform.SetParent(null);
                mec.transform.position = new Vector2(8.0f, chaserYAxis);
                mec.SetActive(true);
                curEnemyCount++;
                yield return Cashing.YieldInstruction.WaitForSeconds(4);
            }
        }
    }
    #endregion

    private async Task MatchStart(int min = 2)
    {
        var matchMakingTicket = await HughServer.GetInstance.Socket.AddMatchmakerAsync("*", min, min);
        ticket = matchMakingTicket.Ticket;
    }

    private void SpawnPlayer(string matchId, IUserPresence user, int spawnIndex = -1)
    {
        if (playerDictionary.ContainsKey(user.SessionId))
        {
            return;
        }

        var spawns = spawnIndex == -1 ?
            spawnPoint.transform.GetChild(Random.Range(0, spawnPoint.transform.childCount))
        : spawnPoint.transform.GetChild(spawnIndex);


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

        //player를 다 생성시켰으면 enemy를 소환한다.
        SpanwEnemyInMultiplay();
    }

    public async void LocalPlayerDied(GameObject player)
    {
        await SendMatchStateAsync(OpCodes.Died, MatchDataJson.Died(player.transform.position));

        playerDictionary.Remove(localUser.SessionId);
        Destroy(player, 0.5f);
    }
    public async Task QuickMatch()
    {
        await HughServer.GetInstance.Socket.LeaveMatchAsync(currentMatch);

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
    private async void OnRecivedMatchMakerMatched(IMatchmakerMatched matchmakerMatched)
    {
        // localuser 캐싱
        localUser = matchmakerMatched.Self.Presence;
        var match = await HughServer.GetInstance.Socket.JoinMatchAsync(matchmakerMatched);

#if UNITY_EDITOR
        Debug.Log("Our Session Id: " + match.Self.SessionId);
#endif

        foreach (var user in match.Presences)
        {
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

            if (playerDictionary.ContainsKey(user.SessionId))
            {
                Destroy(playerDictionary[user.SessionId]);
                playerDictionary.Remove(user.SessionId);
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
                var playerToDestroy = playerDictionary[userSessionId];
                Destroy(playerToDestroy, 0.5f);
                playerDictionary.Remove(userSessionId);
                if (playerDictionary.Count == 0)
                {
                    await QuickMatch();
                }
                break;
            case OpCodes.SpawnPlayer:
                SpawnPlayer(currentMatch.Id, matchState.UserPresence, int.Parse(state["spawnIndex"]));
                break;
            case OpCodes.Score:
                multiplayManager.UpdateScoreInServer(int.Parse(state["multiScore"]));
                break;
            default:
                break;
        }

    }

    public async Task SendMatchStateAsync(long opCode, string state)
    {
        await HughServer.GetInstance.Socket.SendMatchStateAsync(currentMatch.Id, opCode, state);
    }

    public void SendMatchState(long opCode, string state)
    {
        HughServer.GetInstance.Socket.SendMatchStateAsync(currentMatch.Id, opCode, state);
    }

}
