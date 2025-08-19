using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MarchingWorld : MonoBehaviour
{
    public GameObject chunkPrefab;
    [SerializeField] public int chunkSize = 16;
    [SerializeField] public int chunksX = 4;
    [SerializeField] public int chunksY = 2;
    [SerializeField] public int seed;
    [SerializeField] bool randomSeed;

    [Header("Map")]
    [SerializeField] bool randomShapeEnabled;
    [SerializeField] int defaultSelectedMap;
    [SerializeField] MapHolder map;

    private Texture2D mapTexture;

    [Header("Visuals")]
    [SerializeField] Material mainMat;
    [SerializeField] Material grabbedGroundMat;
    [SerializeField] Material particleMat;

    [SerializeField] List<Color> grassColors;
    [SerializeField] List<Color> groundPrimaryColors;
    [SerializeField] List<Color> groundSecondaryColors;
    [SerializeField] List<Color> spotColors;

    [SerializeField] bool randomColorEnabled;

    [Header("Particles")]
    [SerializeField] string particleName;
    [SerializeField] string audioName;
    [SerializeField] float particleRadiusAdjustment;

    private string grassString = "_GrassColor";
    private string groundPrimaryString = "_TopColor";
    private string groundSecondaryString = "_BottomColor";
    private string spotString = "_SpotColor";

    private Dictionary<Vector3Int, CubesMarching> chunks = new();
    private readonly Queue<CubesMarching> pendingMeshUpdates = new();

    private void FixedUpdate()
    {
        UpdateMeshes();
    }
    public void ForceUpdateAllMeshes()
    {
        foreach(var kvp in chunks)
        {
            kvp.Value.SetMesh();
        }
    }
    public void UpdateMeshes()
    {
        lock (pendingMeshUpdates)
        {
            Queue<CubesMarching> notUpdatedMeshes = new();
            while (pendingMeshUpdates.Count > 0)
            {
                var chunk = pendingMeshUpdates.Dequeue();
                if(!chunk.SetMesh())
                {
                    notUpdatedMeshes.Enqueue(chunk);
                }
            }

            while(notUpdatedMeshes.Count > 0)
            {
                pendingMeshUpdates.Enqueue(notUpdatedMeshes.Dequeue());
            }
        }
    }
    public void RegenerateWorld()
    {
        foreach(var chunkKVP in chunks)
        {
            Destroy(chunkKVP.Value.gameObject);
        }
        chunks.Clear();

        for (int x = 0; x < chunksX; x++)
        {
            for (int y = 0; y < chunksY; y++)
            {
                for (int z = 0; z < chunksX; z++)
                {
                    Vector3Int chunkPos = new Vector3Int(x, y, z);
                    Vector3 worldPos = new Vector3(x, y, z) * (chunkSize);

                    GameObject chunkGO = Instantiate(chunkPrefab, worldPos, Quaternion.identity, transform);
                    var chunk = chunkGO.GetComponent<CubesMarching>();

                    chunks[chunkPos] = chunk;
                }
            }
        }

        foreach (var chunk in chunks)
        {
            chunk.Value.Initialize(chunk.Key * chunkSize, seed, mapTexture, chunkSize, chunkSize * chunksX, chunkSize, chunkSize * chunksY);
        }

        foreach (var chunk in chunks)
        {
            chunk.Value.gameObject.GetComponent<Renderer>().material = mainMat;
        }
    }
    public void GenerateWorld()
    {
        foreach(var chunk in chunks)
        {
            if(chunk.Value != null)
            {
                Destroy(chunk.Value.gameObject);
            }
        }
        chunks.Clear();

        if (randomShapeEnabled)
        {
            mapTexture = map.maps[Random.Range(0, map.maps.Count)];
        }
        else if (map.maps.Count > defaultSelectedMap)
        {
            mapTexture = map.maps[defaultSelectedMap];
        }
        else
        {
            mapTexture = map.maps[0];
        }

        if (Application.isPlaying)
        {
            ObjectManager.Instance?.SetTextureSpawnLocations(mapTexture);
        }
        if (randomColorEnabled)
        {
            mainMat = new Material(mainMat);

            Color grassColor = grassColors[Random.Range(0, grassColors.Count)];
            Color groundPrimaryColor = groundPrimaryColors[Random.Range(0, groundPrimaryColors.Count)];
            Color groundSecondaryColor = groundSecondaryColors[Random.Range(0, groundSecondaryColors.Count)];
            Color spotColor = spotColors[Random.Range(0, spotColors.Count)];

            mainMat.SetColor(grassString, grassColor);
            mainMat.SetColor(groundPrimaryString, groundPrimaryColor);
            mainMat.SetColor(groundSecondaryString, groundSecondaryColor);
            mainMat.SetColor(spotString, spotColor);
            
            grabbedGroundMat.SetColor(grassString, grassColor);
            grabbedGroundMat.SetColor(groundPrimaryString, groundPrimaryColor);
            grabbedGroundMat.SetColor(groundSecondaryString, groundSecondaryColor);
            grabbedGroundMat.SetColor(spotString, spotColor);

            particleMat.SetColor(grassString, grassColor);
            particleMat.SetColor(groundPrimaryString, groundPrimaryColor);
        }
        if (randomSeed)
        {
            seed = Random.Range(0, 1000);
        }

        mainMat = new(mainMat);

        RegenerateWorld();
    }

    public async Task<bool[,,]> DeformedSphereReturnValues(Vector3 worldPos, float radius, float intensity)
    {
        List<ChunkDeformationResult> results = new();

        List<Task<(List<(Vector3Int worldPos, bool value)>, Vector3Int min, Vector3Int max)>> chunkTasks = new();

        foreach (var kv in chunks)
        {
            Vector3Int chunkOrigin = kv.Key * chunkSize;
            CubesMarching chunk = kv.Value;

            if (!chunk.bounds.Intersects(new Bounds(worldPos, Vector3.one * (radius + 1) * 2f)))
                continue;

            // Run deformation in a Task
            chunkTasks.Add(Task.Run(() =>
            {
                var result = new ChunkDeformationResult();
                result.chunk = chunk;


                var values = new List<(Vector3Int worldPos, bool value)>();
                Vector3Int min = new(int.MaxValue, int.MaxValue, int.MaxValue);
                Vector3Int max = new(int.MinValue, int.MinValue, int.MinValue);

                lock (chunk)
                {

                    bool[,,] localHeights = chunk.DeformSphereReturnValues(worldPos, radius, intensity);

                    int sx = localHeights.GetLength(0);
                    int sy = localHeights.GetLength(1);
                    int sz = localHeights.GetLength(2);

                    for (int x = 0; x < sx; x++)
                    {
                        for (int y = 0; y < sy; y++)
                        {
                            for (int z = 0; z < sz; z++)
                            {
                                bool value = localHeights[x, y, z];

                                //if(!localHeights[x, y, z] && !localHeights[Mathf.Max(x-1, 0), Mathf.Max(y - 1, 0), Mathf.Max(y - 1, 0)]) continue;

                                Vector3Int worldVoxel = chunkOrigin + new Vector3Int(x, y, z);



                                values.Add((worldVoxel, value));


                                min = Vector3Int.Min(min, worldVoxel);
                                max = Vector3Int.Max(max, worldVoxel);
                            }
                        }
                    }


                }
                lock (results) results.Add(result);

                return (values, min, max);
            }));
        }

        // Wait for all chunk tasks to finish
        var chunkResults = await Task.WhenAll(chunkTasks);

        List<(Vector3Int worldPos, bool value)> allValues = new();
        Vector3Int minGlobal = new(int.MaxValue, int.MaxValue, int.MaxValue);
        Vector3Int maxGlobal = new(int.MinValue, int.MinValue, int.MinValue);

        foreach (var (values, min, max) in chunkResults)
        {
            allValues.AddRange(values);
            minGlobal = Vector3Int.Min(minGlobal, min);
            maxGlobal = Vector3Int.Max(maxGlobal, max);
        }

        Vector3Int size = maxGlobal - minGlobal + Vector3Int.one;

        int padding = 1;

        Vector3Int paddedSize = new(size.x + padding * 2, size.y + padding * 2, size.z + padding * 2);
        bool[,,] result = new bool[paddedSize.x, paddedSize.y, paddedSize.z];

        foreach (var (worldVoxel, value) in allValues)
        {
            Vector3Int localIndex = worldVoxel - minGlobal + new Vector3Int(padding, padding, padding);
            result[localIndex.x, localIndex.y, localIndex.z] = value;
        }

        lock (pendingMeshUpdates)
        {
            foreach (var outChunk in results)
            {
                if (!pendingMeshUpdates.Contains(outChunk.chunk))
                {
                    pendingMeshUpdates.Enqueue(outChunk.chunk);
                }
            }
        }

        DoEffectRequest(worldPos, radius);

        return (CleanReturnBools(result));
    }
    private bool[,,] CleanReturnBools(bool[,,] source)
    {
        int padding = 1;

        Vector3Int min = new(int.MaxValue, int.MaxValue, int.MaxValue);
        Vector3Int max = new(int.MinValue, int.MinValue, int.MinValue);
        int sx = source.GetLength(0);
        int sy = source.GetLength(1);
        int sz = source.GetLength(2);

        bool foundAny = false;

        for (int x = 0; x < sx; x++)
        {
            for (int y = 0; y < sy; y++)
            {
                for (int z = 0; z < sz; z++)
                {
                    if (source[x, y, z])
                    {
                        min = Vector3Int.Min(min, new Vector3Int(x, y, z));
                        max = Vector3Int.Max(max, new Vector3Int(x, y, z));

                        foundAny = true;
                    }
                }
            }
        }

        if(!foundAny) return new bool[0,0,0];

        min.x = Mathf.Max(min.x - padding, 0);
        min.y = Mathf.Max(min.y - padding, 0);
        min.z = Mathf.Max(min.z - padding, 0);

        max.x = Mathf.Min(max.x + padding, sx - 1);
        max.y = Mathf.Min(max.y + padding, sy - 1);
        max.z = Mathf.Min(max.z + padding, sz - 1);

        // 3. Compute size
        Vector3Int size = max - min + Vector3Int.one;
        bool[,,] trimmed = new bool[size.x, size.y, size.z];

        // 4. Copy values
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    trimmed[x, y, z] = source[min.x + x, min.y + y, min.z + z];
                }
            }
        }

        return trimmed;
    }
    public async Task DeformSphereAt(Vector3 worldPos, float radius, float intensity)
    {
        List<ChunkDeformationResult> results = new();
        List<Task> tasks = new();

        foreach (var kv in chunks)
        {
            var chunk = kv.Value;
            lock (chunk)
            {
                if (chunk.bounds.Intersects(new Bounds(worldPos, Vector3.one * (radius + 1) * 2f)))
                {
                    var localChunk = chunk;
                    tasks.Add(Task.Run(() =>
                    {
                        var result = new ChunkDeformationResult();
                        result.chunk = localChunk;
                        result.deformed = result.chunk.DeformSphere(worldPos, radius, intensity);
                        lock (results) results.Add(result);
                    }));
                }
            }
        }

        await Task.WhenAll(tasks);

        DoEffectRequest(worldPos, radius);

        bool atLeastOneTrue = results.Any(item => item.deformed);
        if (!atLeastOneTrue)
            return;

        lock (pendingMeshUpdates)
        {
            foreach (var result in results)
            {
                if (!pendingMeshUpdates.Contains(result.chunk) && result.deformed)
                {
                    pendingMeshUpdates.Enqueue(result.chunk);
                }
            }
        }
    }


    public async Task DeformCylinderAt(Vector3 worldPos, float radius, float height, float intensity)
    {
        List<ChunkDeformationResult> results = new();
        List<Task> tasks = new();

        foreach (var kv in chunks)
        {
            var chunk = kv.Value;
            lock (chunk)
            {
                if (chunk.bounds.Intersects(new Bounds(worldPos, new Vector3(radius * 2 + 2, height * 2 + 2, radius * 2 + 2))))
                {
                    var localChunk = chunk;
                    tasks.Add(Task.Run(() =>
                    {
                        var result = new ChunkDeformationResult();
                        result.chunk = localChunk;
                        result.deformed = chunk.DeformCylinder(worldPos, radius, height, intensity);
                        lock (results) results.Add(result);
                    }));
                }
            }
        }

        await Task.WhenAll(tasks);

        bool atLeastOneTrue = results.Any(item => item.deformed);
        if (!atLeastOneTrue)
            return;

        DoEffectRequest(worldPos, radius);

        lock (pendingMeshUpdates)
        {
            foreach (var result in results)
            {
                if (!pendingMeshUpdates.Contains(result.chunk) && result.deformed)
                {
                    pendingMeshUpdates.Enqueue(result.chunk);
                }
            }
        }
    }

    public async Task DeformCubeAt(Bounds bounds, float intensity)
    {
        //Debug.Log("Deforming CUbe!! min" + bounds.min + " max: " + bounds.max + " center: "+ bounds.center);
        List<ChunkDeformationResult> results = new();
        List<Task> tasks = new();

        foreach (var kv in chunks)
        {
            var chunk = kv.Value;
            lock (chunk)
            {
                if (chunk.bounds.Intersects(bounds))
                {
                    var localChunk = chunk;
                    tasks.Add(Task.Run(() =>
                    {
                        var result = new ChunkDeformationResult();
                        result.chunk = localChunk;
                        result.deformed = chunk.DeformCube(bounds.center, bounds.size.x -1 , bounds.size.y -1 , bounds.size.z -1, intensity);
                        lock (results) results.Add(result);
                    }));
                }
            }
        }

        await Task.WhenAll(tasks);

        bool atLeastOneTrue = results.Any(item => item.deformed);
        if (!atLeastOneTrue)
            return;

        DoEffectRequest(bounds.center, Vector3.Magnitude(new Vector3(bounds.size.x, bounds.size.y, bounds.size.z)));

        lock (pendingMeshUpdates)
        {
            foreach (var result in results)
            {
                if (!pendingMeshUpdates.Contains(result.chunk) && result.deformed)
                {
                    pendingMeshUpdates.Enqueue(result.chunk);
                }
            }
        }
    }

    void DoEffectRequest(Vector3 position, float radius)
    {
        ParticleManager.Instance?.DoParticles(particleName, position, Vector3.one * radius * particleRadiusAdjustment);
        AudioManager.Instance.Play3DAudioAtPosition(audioName, position, true);
    }

    class ChunkDeformationResult
    {
        public CubesMarching chunk;
        public float[,,] newHeights;
        public bool deformed;
    }
}