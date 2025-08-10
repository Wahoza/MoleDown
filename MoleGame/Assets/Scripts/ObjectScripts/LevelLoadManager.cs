using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadManager : MonoBehaviour
{
    public static LevelLoadManager Instance;
    [SerializeField] private Transform spawnOrigin;
    [SerializeField] private float spawnRadius;

    [Header("DissapearLoadingScreen")]
    [SerializeField] GameObject loadingScreen;
    [SerializeField] float heightChange;
    [SerializeField] float moveUpDuration;

    [Header("PlayerCreation")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject spawnPrefab;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void LoadLevel()
    {
        Time.timeScale = 0;

        LevelMarchingPlatform.Instance.GenerateWorld();

        //SpawnPlayers
        var playerConfigs = PlayerConfigManager.Instance.GetPlayerConfigs();

        float angleStep = 360f / playerConfigs.Count;

        var players = new Dictionary<PlayerConfig, GameObject>();

        for (int i = 0; i < playerConfigs.Count; i++)
        {
            float angleDeg = i * angleStep;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            float x = spawnOrigin.position.x + spawnRadius * Mathf.Cos(angleRad);
            float z = spawnOrigin.position.z + spawnRadius * Mathf.Sin(angleRad);
            float y = spawnOrigin.position.y; // stays the same since it's the XZ plane

            var spawnPoint = Instantiate(spawnPrefab, transform);
            spawnPoint.transform.position = new Vector3(x, y, z);

            var player = Instantiate(playerPrefab, spawnPoint.transform);

            players.Add(playerConfigs[i],  player);
        }

        Physics.SyncTransforms();

        foreach (var player in players)
        {
            player.Value.GetComponent<PlayerInputHandler>().Initialize(player.Key);
        }

        UIManager.Instance.scoreBoardHolder.gameObject.SetActive(false);

    }

    public void CompleteLoad()
    {
        StartCoroutine(CompleteLoadRoutine());
    }
    private IEnumerator CompleteLoadRoutine()
    {
        GameManager.Instance.StartRound();

        yield return loadingScreen.transform.DOLocalMoveY(heightChange, moveUpDuration).SetEase(Ease.InSine).SetUpdate(UpdateType.Normal ,true).WaitForCompletion();

        loadingScreen.gameObject.SetActive(false);
    }
}
