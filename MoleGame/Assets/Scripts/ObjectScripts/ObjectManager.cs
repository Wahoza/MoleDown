using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;


public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }
    private List<GameObject> objects = new();

    [SerializedDictionary("ObjectPrefab", "SpawnTop")]
    public SerializedDictionary<GameObject, bool> objectSpawnDictionary;

    public List<GameObject> powerUpList;
    private List<GameObject> powerupsThisRound;

    [Header("SpawnRateSettings")]
    [SerializeField] float minSpawnRate;
    [SerializeField] float maxSpawnRate;
    [SerializeField] float minSpawnRateIncrease;
    [SerializeField] float maxSpawnRateIncrease;

    [Header("PowerupSpawnSettings")]
    [SerializeField] float minPowerupSpawnRate;
    [SerializeField] float maxPowerupSpawnRate;
    [SerializeField] float powerupSpawnRateIncrease;
    [SerializeField] float powerupSpawnChance;
    [SerializeField] float powerupRoundChance;
    [SerializeField] float chunkStepSize;
    [SerializeField] float textureMaxValue;

    [Header("SpawnLocations")]
    [SerializeField] BoxCollider spawnAreaTop;
    [SerializeField] Transform spawnBottomHeight;
    [SerializeField] float spawnBottomRadius;

    [SerializeField] Texture2D defaulTexture;

    private List<Vector3> possibleSpawnLocations = new();
    private Coroutine spawnObjectRoutine;
    private Coroutine spawnPowerupObjectRoutine;

    private float currentSpawnRate;
    private float currentPowerupSpawnRate;
    private float currentSpawnRateIncrease;
    private float currentPowerupSpawnRateIncrease;

    private Dictionary<GameObject, bool> currentObjectDictionary = new();
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            SetTextureSpawnLocations(defaulTexture);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetTextureSpawnLocations(Texture2D texture)
    {
        possibleSpawnLocations.Clear();

        var texWidth = texture.width;
        var texHeight = texture.height;

        for (float u = 0; u < texWidth; u += chunkStepSize)
        {
            for (float v = 0; v < texHeight; v += chunkStepSize)
            {
                var normalizedU = u / texWidth;
                var normalizedV = v / texHeight;
                if(texture.GetPixelBilinear(normalizedU, normalizedV).r < textureMaxValue)
                {
                    var adjustedPosition = -spawnAreaTop.size / 2;
                    adjustedPosition += spawnAreaTop.transform.position;
                    Vector3 areaSize = spawnAreaTop.size;
                    possibleSpawnLocations.Add(new Vector3(normalizedU * areaSize.x, spawnAreaTop.transform.position.y, normalizedV * areaSize.z));
                }
            }
        }
    }
    public void StartNewRound()
    {
        currentObjectDictionary.Clear();

        currentObjectDictionary = GetRandomEntries(objectSpawnDictionary, Random.Range(1, objectSpawnDictionary.Count));

        currentSpawnRate = minSpawnRate;
        currentSpawnRateIncrease = Random.Range(minSpawnRateIncrease, maxSpawnRateIncrease);


        if (spawnObjectRoutine == null)
        {
            spawnObjectRoutine = StartCoroutine(SpawnObjectCoroutine());
        }

        currentPowerupSpawnRate = minPowerupSpawnRate;
        currentPowerupSpawnRateIncrease = powerupSpawnRateIncrease;

        powerupsThisRound = GetRandomEntries(powerUpList, Random.Range(1, powerUpList.Count));

        if(Random.value <= powerupRoundChance)
        {
            Debug.Log("Powerup Round!");
            if(spawnPowerupObjectRoutine == null && possibleSpawnLocations.Count > 0)
            {
                Debug.Log("Powerup Initiated");
                spawnPowerupObjectRoutine = StartCoroutine(SpawnPowerupCoroutine());
            }
        }
    }
    IEnumerator SpawnPowerupCoroutine()
    {
        while (true)
        {
            float delay = 1f / currentPowerupSpawnRate;
            Debug.Log(delay + "PowerupDelay");
            yield return new WaitForSeconds(delay);

            currentPowerupSpawnRate += currentPowerupSpawnRateIncrease;

            currentPowerupSpawnRate = currentPowerupSpawnRate > maxPowerupSpawnRate ? maxPowerupSpawnRate : (currentPowerupSpawnRate < minPowerupSpawnRate ? minPowerupSpawnRate : currentPowerupSpawnRate);

            SpawnPowerup();
        }
    }
    IEnumerator SpawnObjectCoroutine()
    {
        while (true)
        {
            float delay = 1f / currentSpawnRate;
            yield return new WaitForSeconds(delay);

            currentSpawnRate += currentSpawnRateIncrease;

            currentSpawnRate = currentSpawnRate > maxSpawnRate ? maxSpawnRate : (currentSpawnRate < minSpawnRate ? minSpawnRate : currentSpawnRate);
            
            SpawnObject();
        }
    }
    void SpawnPowerup()
    {
        if (Random.value > powerupSpawnChance)
            return;

        Debug.Log("Powerup Spawned");

        var objectToSpawn = powerupsThisRound[Random.Range(0, powerupsThisRound.Count)];

        if (objectToSpawn != null)
        {
            var powerup = Instantiate(objectToSpawn);
            powerup.transform.position = possibleSpawnLocations[Random.Range(0, possibleSpawnLocations.Count)];
        }
    }
    void SpawnObject() {
        var objectToSpawn = GetRandomEntries(currentObjectDictionary, 1);

        foreach (var entry in objectToSpawn)
        {
            var obstacleObject = Instantiate(entry.Key);

            if (obstacleObject != null)
            {
                if (entry.Value)
                {
                    var randomVals = Random.ColorHSV();
                    var position = spawnAreaTop.size;

                    position.x *= randomVals.r;
                    position.y *= randomVals.g;
                    position.z *= randomVals.b;

                    position -= spawnAreaTop.size / 2;
                    position += spawnAreaTop.transform.position;

                    obstacleObject.transform.position = position;
                }
                else
                {
                    float angle = Random.Range(0f, Mathf.PI * 2f); // random radians 0–360°
                    float x = Mathf.Cos(angle) * spawnBottomRadius;
                    float z = Mathf.Sin(angle) * spawnBottomRadius;
                    Vector3 center = spawnBottomHeight.position;

                    var spawn = new Vector3(center.x + x, center.y, center.z + z);

                    obstacleObject.transform.position = spawn;
                }
            }
        }
    }
    public void StopSpawning()
    {
        if(spawnObjectRoutine != null)
        {
            StopCoroutine(spawnObjectRoutine);
            spawnObjectRoutine = null;
        }
        if(spawnPowerupObjectRoutine != null)
        {
            StopCoroutine(spawnPowerupObjectRoutine);
            spawnPowerupObjectRoutine = null;
        }
    }
    public void RegisterObject(GameObject obj)
    {
        objects.Add(obj);
    }

    public void DestroyAllObjects()
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        objects.Clear();
    }

    Dictionary<TKey, TValue> GetRandomEntries<TKey, TValue>(Dictionary<TKey, TValue> source, int count)
    {
        // Shuffle the dictionary and take the first `count` items
        return source
            .OrderBy(x => Random.value) // shuffle
            .Take(count)               // pick N
            .ToDictionary(x => x.Key, x => x.Value);
    }
    List<T> GetRandomEntries<T>(List<T> source, int count)
    {
        return source
            .OrderBy(x => Random.value)
            .Take(count)
            .ToList();
    }
}
