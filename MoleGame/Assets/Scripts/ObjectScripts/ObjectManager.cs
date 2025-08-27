using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;


public class ObjectManager : MonoBehaviour
{
    public enum SpawnOptions
    {
        Above,
        Around,
        Below
    }

    public static ObjectManager Instance { get; private set; }
    private List<GameObject> objects = new();

    [SerializedDictionary("ObjectPrefab", "SpawnTop")]
    public SerializedDictionary<GameObject, SpawnOptions> objectSpawnDictionary;

    public List<GameObject> powerUpList;
    public List<GameObject> groundedObstacles;
    private List<GameObject> powerupsThisRound;

    [Header("SpawnRateSettings")]
    [SerializeField] float insaneSpawnRateIncrease;
    [SerializeField] float insaneSpawnIncreaseIncrease;
    [SerializeField] float minSpawnRate;
    [SerializeField] float maxSpawnRate;
    [SerializeField] float minSpawnRateIncrease;
    [SerializeField] float maxSpawnRateIncrease;
    [SerializeField] int maxSpawnCount;

    [Header("PowerupSpawnSettings")]
    [SerializeField] float insanePowerupSpawnRateIncrease;
    [SerializeField] float insanePowerupSpawnIncreaseIncrease;
    [SerializeField] float minPowerupSpawnRate;
    [SerializeField] float maxPowerupSpawnRate;
    [SerializeField] float powerupSpawnRateIncrease;
    [SerializeField] float powerupSpawnChance;
    [SerializeField] float powerupRoundChance;
    [SerializeField] float chunkStepSize;
    [SerializeField] float textureMaxValue;

    [Header("GroundedObject Settings")]
    [SerializeField] int minGroundedObjectCount;
    [SerializeField] int maxGroundedObjectCount;
    [SerializeField] float groundedRoundChance;
    [SerializeField] int maxGroundedVariation;

    [Header("SpawnLocations")]
    [SerializeField] BoxCollider spawnAreaTop;
    [SerializeField] BoxCollider spawnAreaBelow;
    [SerializeField] Transform spawnBottomHeight;
    [SerializeField] float spawnBottomRadius;

    [SerializeField] Texture2D defaulTexture;

    [Header("ValidTerrainDetection")]
    [SerializeField] float raycastHeight;
    [SerializeField] float raycastLength;
    [SerializeField] LayerMask groudLayerMask;



    private List<Vector3> possibleSpawnLocations = new();
    private List<Vector3> validTerrainLocations = new();
    private Coroutine spawnObjectRoutine;
    private Coroutine spawnPowerupObjectRoutine;

    private float currentSpawnRate;
    private float currentPowerupSpawnRate;
    private float currentSpawnRateIncrease;
    private float currentPowerupSpawnRateIncrease;
    
    private Dictionary<GameObject, SpawnOptions> currentObjectDictionary = new();
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

        validTerrainLocations = possibleSpawnLocations;
    }
    public void StartNewRound()
    {
        bool isInsaneRound;
        isInsaneRound = Random.value <= GameSettingsManager.Instance.GetInsaneRoundChance();

        currentObjectDictionary.Clear();

        currentObjectDictionary = GetRandomEntries(objectSpawnDictionary, Random.Range(1, maxSpawnCount));

        currentSpawnRate = minSpawnRate;
        currentSpawnRateIncrease = Random.Range(minSpawnRateIncrease, maxSpawnRateIncrease);

        if (isInsaneRound)
        {
            currentSpawnRate += insaneSpawnRateIncrease;
            currentSpawnRateIncrease += insaneSpawnIncreaseIncrease;
        }

        if (spawnObjectRoutine == null)
        {
            spawnObjectRoutine = StartCoroutine(SpawnObjectCoroutine());
        }

        currentPowerupSpawnRate = minPowerupSpawnRate;
        currentPowerupSpawnRateIncrease = powerupSpawnRateIncrease;

        if (isInsaneRound)
        {
            currentPowerupSpawnRate += insanePowerupSpawnRateIncrease;
            currentPowerupSpawnRateIncrease += insanePowerupSpawnIncreaseIncrease;
        }
        powerupsThisRound = GetRandomEntries(powerUpList, Random.Range(1, powerUpList.Count));

        if(Random.value <= powerupRoundChance || isInsaneRound)
        {
            Debug.Log("Powerup Round!");
            if(spawnPowerupObjectRoutine == null && possibleSpawnLocations.Count > 0)
            {
                Debug.Log("Powerup Initiated");
                spawnPowerupObjectRoutine = StartCoroutine(SpawnPowerupCoroutine());
            }
        }

        if(Random.value <= groundedRoundChance)
        {
            int groundedSelectionCount = Random.Range(1, maxGroundedVariation);
            int groundedTotalCount = Random.Range(minGroundedObjectCount, maxGroundedObjectCount);
            List<GameObject> currentGroundedObjects =GetRandomEntries(groundedObstacles, groundedSelectionCount);

            foreach (var currentGroundedObject in currentGroundedObjects)
            {
                for (int i = 0; i < groundedTotalCount / currentGroundedObjects.Count; i++)
                {
                    var currentObject = Instantiate(currentGroundedObject);
                    currentObject.transform.position = GetRandomValidTerrainPoint();
                }
            }
        }
    }
    IEnumerator SpawnPowerupCoroutine()
    {
        while (true)
        {
            float delay = 1f / currentPowerupSpawnRate;
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

        var objectToSpawn = powerupsThisRound[Random.Range(0, powerupsThisRound.Count)];

        if (objectToSpawn != null)
        {
            var pos = GetRandomValidTerrainPoint();
            if(pos == -Vector3.one)
            {
                return;
            }

            var powerup = Instantiate(objectToSpawn);
            pos.y = spawnAreaTop.transform.position.y;
            powerup.transform.position = pos;//possibleSpawnLocations[Random.Range(0, possibleSpawnLocations.Count)];
        }
    }
    void SpawnObject() {
        var objectToSpawn = GetRandomEntries(currentObjectDictionary, 1);

        foreach (var entry in objectToSpawn)
        {
            var obstacleObject = Instantiate(entry.Key);

            if (obstacleObject != null)
            {
                switch (entry.Value)
                {
                    case SpawnOptions.Above:
                        {
                            var randomVals = Random.ColorHSV();
                            var position = spawnAreaTop.size;

                            position.x *= randomVals.r;
                            position.y *= randomVals.g;
                            position.z *= randomVals.b;

                            position -= spawnAreaTop.size / 2;
                            position += spawnAreaTop.transform.position;

                            obstacleObject.transform.position = position;
                            return;
                        }

                    case SpawnOptions.Around:
                        {
                            float angle = Random.Range(0f, Mathf.PI * 2f); // random radians 0–360°
                            float x = Mathf.Cos(angle) * spawnBottomRadius;
                            float z = Mathf.Sin(angle) * spawnBottomRadius;
                            Vector3 center = spawnBottomHeight.position;

                            var spawn = new Vector3(center.x + x, center.y, center.z + z);

                            obstacleObject.transform.position = spawn;
                            return;
                        }

                    case SpawnOptions.Below:
                        {
                            var randomVals = Random.ColorHSV();
                            var position = spawnAreaBelow.size;

                            position.x *= randomVals.r;
                            position.y *= randomVals.g;
                            position.z *= randomVals.b;

                            position -= spawnAreaBelow.size / 2;
                            position += spawnAreaBelow.transform.position;

                            obstacleObject.transform.position = position;
                        }
                    break;
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

    public Vector3 GetRandomValidTerrainPoint()
    { 
        while(validTerrainLocations.Count > 0)
        {
            int index = Random.Range(0, validTerrainLocations.Count);
            Vector3 pointToCheck = validTerrainLocations[index];
            pointToCheck.y = raycastHeight;
            RaycastHit hit;
            if (Physics.Raycast(pointToCheck, Vector3.down, out hit, raycastLength, groudLayerMask))
            {
                return hit.point;
            }
            else
            {
                validTerrainLocations.RemoveAt(index);
            }
        }

        return -Vector3.one;
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
