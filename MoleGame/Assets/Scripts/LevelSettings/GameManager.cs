using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics.Contracts;
using Mono.Cecil;
using System.Linq;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform gameCamera;
    [SerializeField] CinemachineTargetGroup group;
    [SerializeField] Transform intermissionScoreEvalCamera;
    [field : SerializeField] public Transform worldCenter { get; private set; }
    [SerializeField] float playerCameraRadius;

    [Header("PlayerCreation")]
    [SerializeField] private GameObject playerPointerPrefab; //Create: Points at player with PID
    [SerializeField] private GameObject playerScorePrefab; //Create: Displays player Score on UI
    [SerializeField] private GameObject playerBoardItemPrefab;

    [Header("Countdown Screen")]
    [SerializeField] CountdownComponent countdown;
    [SerializeField] int startCountdownValue;
    [SerializeField] int endCountdownValue;
    [SerializeField] float numberShowDuration;

    [Header("Ending")]
    [SerializeField] int endSceneId;

    [Header("Music")]
    [SerializeField] string menuMusic;

    public static GameManager Instance;

    public bool isPaused;
    private int rounds;
    private int currentRound;
    private bool isFinished;
    private bool isFinishedInput;

    private Dictionary<PlayerConfig, PlayerSetup> players = new();
    private Dictionary<PlayerConfig, int> playerScores = new();
    private List<PlayerConfig> deadPlayers = new();
    private List<PlayerConfig> alivePlayers = new();

    private PlayerInputMap controls;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            controls = new();
            PlayerConfigManager.Instance.GetPlayerConfigs()[0].Input.onActionTriggered += CallFinished;
            rounds = GameSettingsManager.Instance.GetRounds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void JoinPlayer(PlayerConfig config, GameObject player){

        if (!players.ContainsKey(config))
        {
            var pointer = Instantiate(playerPointerPrefab, UIManager.Instance.pointerHolder);
            pointer.GetComponent<PointerScript>().InitializePlayer(config, player.transform);

            var scorer = Instantiate(playerScorePrefab, UIManager.Instance.playerStatsHolder);
            scorer.GetComponent<PlayerStatsComponent>().Initialize(config);
            
            var board = Instantiate(playerBoardItemPrefab, UIManager.Instance.scoreBoardHolder.GetScoreHolder());
            board.GetComponent<ScoreboardItemScript>().Initialize(config);
            board.GetComponent<ScoreboardItemScript>().SetHeight();

            PlayerSetup playerSetup = new PlayerSetup();
            playerSetup.player = player;
            playerSetup.pointer = pointer.GetComponent<PointerScript>();
            playerSetup.stats = scorer.GetComponent<PlayerStatsComponent>();
            playerSetup.scoreboard = board.GetComponent<ScoreboardItemScript>();

            players.Add(config, playerSetup);

        }
    }

    public List<Transform> GetAlivePlayers()
    {
        List<Transform> result = new List<Transform>();
        foreach (var player in alivePlayers)
        {
            result.Add(players[player].player.transform);
        }

        return result;
    }
    public void OnPlayerDie(GameObject player)
    {
        foreach (var playerKVP in players)
        {
            if(playerKVP.Value.player == player)
            {
                OnPlayerDie(playerKVP.Key);
                return;
            }
        }
    }
    public void OnPlayerDie(PlayerConfig player)
    {
        if (!deadPlayers.Contains(player))
        {
            deadPlayers.Add(player);
            alivePlayers.Remove(player);
            group.RemoveMember(players[player].player.transform);
        }

        foreach (var playerKVP in players)
        {
            if(playerKVP.Key == player)
            {
                playerKVP.Value.pointer.ShowDead();
            }
        }
        Debug.Log(deadPlayers.Count + "DeadP");
        Debug.Log(players.Count + "Players");
        if (deadPlayers.Count >= players.Count - 1) //-1 to account for one winner
        {
            FinishRound();
        }
    }
    public void FinishRound()
    {
        Debug.Log("BOAROA");

        intermissionScoreEvalCamera.gameObject.SetActive(true);
        gameCamera.gameObject.SetActive(false);

        playerScores.Clear();

        EvaluateScores();

        ObjectManager.Instance.StopSpawning();

        if (currentRound >= rounds)
        {
            StartCoroutine(DoFinalFinish());
        }
        else
        {
            StartCoroutine(DoFinish());
        }
    }
    private IEnumerator DoFinalFinish()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(endSceneId);
        operation.allowSceneActivation = false;

        yield return StartCoroutine(UIManager.Instance.scoreBoardHolder.Appear());

        operation.allowSceneActivation = true;

        while (!operation.isDone)
            yield return null;
    }
    private IEnumerator DoFinish()
    {
        isFinished = false;

        UIManager.Instance.scoreBoardHolder.gameObject.SetActive(true);
        
        Time.timeScale = 0;

        yield return StartCoroutine(UIManager.Instance.scoreBoardHolder.Appear());

        AudioManager.Instance.SetMusic(menuMusic);
        AudioManager.Instance.PlayMusic();

        var scoreList = players
            .OrderBy(kvp => kvp.Key.finalPoints)
            .Select(kvp => kvp.Value.scoreboard)
            .ToList();

        scoreList.Reverse();

        var scores = players
            .OrderBy(kvp => kvp.Key.finalPoints)
            .Select(kvp => playerScores[kvp.Key])
            .ToList();

        scores.Reverse();

        var finalScores = players
            .OrderBy(kvp => kvp.Key.finalPoints)
            .Select(kvp => kvp.Key.finalPoints)
            .ToList();

        finalScores.Reverse();

        for (int i = 0; i < scoreList.Count; i++)
        {
            scoreList[i].transform.SetSiblingIndex(i);
            scoreList[i].SetHeight();

            scoreList[i].UpdatePoints();
            scoreList[i].SetRoundPoints(scores[i]);

            //Adjust placement
            if(scoreList.Count > 0)
            {
                for(int scoreIterator = i; scoreIterator > 0; scoreIterator--)
                {
                    if(finalScores[scoreIterator] >= finalScores[i])
                    {
                        scoreList[i].SetPlacement(scoreIterator);
                        break;
                    }
                }
            }
            else
            {
                scoreList[i].SetPlacement(i);
            }

             scoreList[i].gameObject.SetActive(true);
        }

        for (int i = scoreList.Count - 1; i >= 0; i--) {
            yield return StartCoroutine(scoreList[i].DoFall());
        }

        deadPlayers.Clear();

        ReloadLevel();


        LevelMarchingPlatform.Instance.UpdateMeshes();

        Physics.simulationMode = SimulationMode.Script;

        // Run one step manually using the fixed timestep
        Physics.Simulate(Time.fixedDeltaTime);

        Physics.simulationMode = SimulationMode.FixedUpdate;

        UIManager.Instance.SetCurrentRound(currentRound + 1, rounds);

        isFinishedInput = false;

        yield return new WaitUntil(() => isFinishedInput);

        for (int i = 0; i < scoreList.Count; i++)
        {
            yield return StartCoroutine(scoreList[i].DoRise());
            scoreList[i].gameObject.SetActive(false);
        }

        yield return StartCoroutine(UIManager.Instance.scoreBoardHolder.Disapear());


        StartRound();
    }

    private void CallFinished(CallbackContext ctx)
    {
        if(isFinishedInput) { return; }

        if (ctx.phase == InputActionPhase.Started || ctx.phase == InputActionPhase.Performed && ctx.action.name == controls.UI.Submit.name)
        {

            Debug.Log("Finished");

            isFinishedInput = true;
        }
    }
    void EvaluateScores()
    {
        if (GameSettingsManager.Instance.GetScoreSettings() == GameSettingsManager.ScoringSettings.Winner)
        {
            int score = players.Count;
            
            if(alivePlayers.Count > 0)
            {
                playerScores.Add(alivePlayers[0], score);
            }
            else
            {
                playerScores.Add(deadPlayers[deadPlayers.Count - 1], score);
            }

            foreach(var player in players)
            {
                if (!playerScores.ContainsKey(player.Key))
                {
                    playerScores[player.Key] = 0;
                }
            }
        }
        else
        {
            int score = 0;
            for (int i = 0; i < deadPlayers.Count; i++)
            {
                score = i;
                playerScores.Add(deadPlayers[i], score);
            }

            if(alivePlayers.Count > 0)
            {
                foreach(var alivePlayer in alivePlayers)
                {
                    if (!playerScores.ContainsKey(alivePlayer))
                    {
                        playerScores.Add(alivePlayer, score + 1);
                    }
                }
            }
        }

        foreach(var score in playerScores)
        {
            score.Key.finalPoints += score.Value;
        }
    }
    public void StartRound()
    {
        deadPlayers.Clear();

        foreach(var player in players)
        {
            alivePlayers.Add(player.Key);
            group.AddMember(player.Value.player.transform, 1, playerCameraRadius);
        }

        gameCamera.gameObject.SetActive(true);
        intermissionScoreEvalCamera.gameObject.SetActive(false);

        countdown.StartCountdown(startCountdownValue, endCountdownValue, numberShowDuration);

        currentRound++;

        ObjectManager.Instance.StartNewRound();
        UIManager.Instance.SetCurrentRound(currentRound, rounds);
    }

    public void ReloadLevel()
    {
        LevelMarchingPlatform.Instance.RegenerateWorld();
        ObjectManager.Instance.DestroyAllObjects();

        var scoreList = players
            .OrderBy(kvp => kvp.Key.finalPoints)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var player in players)
        {
            player.Value.player.transform.localPosition = Vector3.zero;
            player.Value.player.transform.localRotation = Quaternion.identity;

            if(player.Key == scoreList[scoreList.Count-1] && scoreList[scoreList.Count - 1].finalPoints != scoreList[scoreList.Count - (scoreList.Count == 1 ? 1 : 2)].finalPoints)
            {
                player.Value.player.GetComponent<PlayerModelControllerComponent>().ShowWinObjects();
            }
            else
            {
                player.Value.player.GetComponent<PlayerModelControllerComponent>().HideWinObjects();
            }

            player.Value.player.GetComponent<PlayerActionHandler>().OnSpawn();

            player.Value.stats.UpdateStats();
            player.Value.pointer.ShowAlive();
        }
        
        Physics.SyncTransforms();

        foreach (var player in players)
        {
            player.Value.player.GetComponentInChildren<SpawnPlatformCreator>().SpawnPlatform(false);
        }
    }
    private void OnDestroy()
    {
        PlayerConfigManager.Instance.GetPlayerConfigs()[0].Input.onActionTriggered -= CallFinished;
    }
    private struct PlayerSetup
    {
        public GameObject player;
        public PointerScript pointer;
        public PlayerStatsComponent stats;
        public ScoreboardItemScript scoreboard;
    }
}
