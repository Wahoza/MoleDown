using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]


public class CubesMarching : MonoBehaviour
{
    [Header("Marching Cubes")]
    [SerializeField] private int m_width = 10;
    [SerializeField] private int m_height = 10;


    [SerializeField] private float heightThreshold = 0.5f;
  
    [SerializeField] private float noiseResulution = 1;


    [SerializeField] private bool visualizeNoise;

    private float[,,] heights;

    //lists due to size uncertanties
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    private MeshFilter meshfilter;
    private MeshCollider meshcollider;

    [Header("Shape")]
    [SerializeField] float falloffStrenght;
    [SerializeField] float falloffPower;
    [SerializeField] float falloffThreshold;
    [SerializeField] int defaultHeightOffset;
    [SerializeField] float defaultHeightScale;

    [Header("Randomness")]
    [SerializeField] int seed;
    [SerializeField] bool randomEnabled;

    float seededX;
    float seededZ;

    [Header("Map")]
    [SerializeField] bool randomShapeEnabled;
    [SerializeField] int defaultSelectedMap;
    [SerializeField] MapHolder map;

    [Header("Testing")]
    [SerializeField] bool debugTick = false;

    Texture2D mapTexture;

    private int m_worldWidth;
    private int m_worldHeight;

    private Vector3Int worldPosition;

    public Bounds bounds {  get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public virtual void Start()
    //{
    //    if(!randomEnabled)
    //        Random.InitState(seed);


    //    if (randomShapeEnabled)
    //    {
    //        mapTexture = map.maps[Random.Range(0, map.maps.Count)];
    //    } 
    //    else if (map.maps.Count > defaultSelectedMap)
    //    {
    //        mapTexture = map.maps[defaultSelectedMap];
    //    }


    //    meshfilter = GetComponent<MeshFilter>();
    //    meshcollider = GetComponent<MeshCollider>();

    //    seededX = Random.value * 1000;
    //    seededZ = Random.value * 1000;

    //    SetHeights();
    //    MarchCubes();
    //    SetMesh();
    //    //meshfilter = GetComponent<MeshFilter>();
    //    //meshcollider = GetComponent<MeshCollider>();
    //    //StartCoroutine(UpdateAll());
    //}

    public void Initialize(Vector3Int worldChunkPos, int worldSeed, Texture2D worldMap, int width, int worldWidth, int height, int worldHeight)
    {
        Random.InitState(worldSeed);
        m_width = width;
        m_height = height;

        m_worldWidth = worldWidth;
        m_worldHeight = worldHeight;
        worldPosition = worldChunkPos;

        mapTexture = worldMap;


        meshfilter = GetComponent<MeshFilter>();
        meshcollider = GetComponent<MeshCollider>();

        seededX = Random.value * 1000;
        seededZ = Random.value * 1000;

        //Debug.Log(worldPosition);
        bounds = new(center: worldPosition + new Vector3((float)width / 2, (float)height / 2, (float)width / 2),  size: new Vector3((float) width, (float) height, (float) width));
        SetHeights();
        MarchCubes();
        SetMesh();
    }
    
    public void SetMesh()
    {
        lock (vertices) lock (triangles)
        {
            if (vertices.Count >= 3 && triangles.Count % 3 == 0 && triangles.All(i => i >= 0 && i < vertices.Count))
            {
                Mesh mesh = new Mesh();

                mesh.vertices = vertices.ToArray();
                mesh.triangles = triangles.ToArray();

                mesh.RecalculateNormals();

                meshfilter.mesh = mesh;
                meshcollider.sharedMesh = mesh;
            }
            else
            {
                //Debug.LogWarning("Invalid mesh data: check vertex and triangle count.");
                MarchCubes();
            }
        }
    }

    private void SetHeights()
    {

        heights = new float[m_width + 1, m_height + 1, m_width + 1];

        for (int x = 0; x < m_width+1; x++)
        {
            for (int y = 0; y < m_height+1; y++)
            {
                for(int z = 0; z < m_width +1 ; z++)
                {
                    
                    float currentHeight = -worldPosition.y + m_worldHeight * Mathf.PerlinNoise((x + worldPosition.x) * noiseResulution + seededX, (z + worldPosition.z) * noiseResulution + seededZ) * defaultHeightScale + defaultHeightOffset;
                    //old radial falloff
                    //float centerX = m_width / 2.0f;
                    //float centerZ = m_width / 2.0f;

                    //float dx = x - centerX;
                    //float dz = z - centerZ;

                    //float dist = Mathf.Sqrt(dx * dx + dz * dz);

                    //float falloff;

                    //if (dist > falloffThreshold)
                    //{
                    //    float d = dist - falloffThreshold;
                    //    falloff = Mathf.Pow(d, falloffPower) * falloffStrenght;

                    //    currentHeight -= falloff;
                    //}

                    float u = ((float)x + (float)worldPosition.x) / m_worldWidth + 1;
                    float v = ((float)z + (float)worldPosition.z) / m_worldWidth + 1;
                    float grayscale = mapTexture.GetPixelBilinear(u, v).grayscale;

                    float curvedFalloff = Mathf.Pow(grayscale, falloffPower);
                    currentHeight -= curvedFalloff * falloffStrenght;
                    float distToSurface; 

                    if(y <= currentHeight - 0.5f)
                    {
                        distToSurface = 0f;
                    }
                    else if(y > currentHeight + 0.5f)
                    {
                        distToSurface = 1f;
                    }
                    else if (y > currentHeight)
                    {
                        distToSurface = y - currentHeight;
                    }
                    else
                    {
                        distToSurface = currentHeight - y;
                    }
                    //Debug.Log(distToSurface);
                        heights[x, y, z] = distToSurface;
                }
            }
        }
    }

    private void MarchCubes()
    {
        lock (vertices) lock (triangles)
        {
            vertices.Clear();
            triangles.Clear();
        }

        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_height; y++)
            {
                for (int z = 0; z < m_width; z++)
                {
                    float[] cubeCorneres = new float[8];


                    for(int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                        cubeCorneres[i] = heights[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), GetConfigurationIndex(cubeCorneres));
                }
            }
        }
    }

    private void MarchCube(Vector3 position, int configIndex)
    {
        if(configIndex == 0 || configIndex == 255)
        {
            return;
        }

        int edgeIndex = 0;
        lock (vertices) lock (triangles)
        {
            for (int t = 0; t < 5; t++)
            {
                for (int v = 0; v < 3; v++)
                {
                    int triTableValue = MarchingTable.Triangles[configIndex, edgeIndex];

                    if (triTableValue == -1)
                    {
                        return;
                    }

                    Vector3 edgeStart = position + MarchingTable.Edges[triTableValue, 0];
                    Vector3 edgeEnd = position + MarchingTable.Edges[triTableValue, 1];

                    Vector3 vertex = (edgeStart + edgeEnd) / 2;

                    vertices.Add(vertex);
                    triangles.Add(vertices.Count - 1);

                    edgeIndex++;
                }
            }
        }
    }
    private int GetConfigurationIndex(float[] cubeCorners)
    {
        int configIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            if(cubeCorners[i] > heightThreshold)
            {
                configIndex |= 1 << i;
            }
        }

        return configIndex;
    }
    public void DeformSphere(Vector3 worldDeformPosition, float radius, float intensity)
    {
        bool modified = false;

        Vector3 localCenter = worldDeformPosition - worldPosition;

        int minX = Mathf.FloorToInt(localCenter.x - radius);
        int maxX = Mathf.CeilToInt(localCenter.x + radius);

        int minY = Mathf.FloorToInt(localCenter.y - radius);
        int maxY = Mathf.CeilToInt(localCenter.y + radius);

        int minZ = Mathf.FloorToInt(localCenter.z - radius);
        int maxZ = Mathf.CeilToInt(localCenter.z + radius);

        for (int x = minX; x <= maxX; x++)
        {
            if (x < 0 || x > m_width) continue;
            for (int y = minY; y <= maxY; y++)
            {
                if (y < 0 || y > m_height) continue;
                for (int z = minZ; z <= maxZ; z++)
                {
                    if (z < 0 || z > m_width) continue;

                    Vector3 point = new Vector3(x, y, z);
                    float dist = Vector3.Distance(point, localCenter);

                    if (dist <= radius)
                    {
                        modified = true;
                        float falloff = 1f - (dist / radius); // smooth falloff
                        heights[x, y, z] = Mathf.Clamp01(heights[x, y, z] - intensity * falloff);
                    }
                }
            }
        }
        if (!modified) return;
        //Debug.Log("Modifications: " + modCounter);

        MarchCubes();
    }

    public bool[,,] DeformSphereReturnValues(Vector3 worldDeformPosition, float radius, float intensity)
    {
        bool[,,] returnHeights = new bool[m_width + 1, m_height + 1, m_width + 1];

        bool modified = false;

        Vector3 localCenter = worldDeformPosition - worldPosition;

        int minX = Mathf.FloorToInt(localCenter.x - radius);
        int maxX = Mathf.CeilToInt(localCenter.x + radius);

        int minY = Mathf.FloorToInt(localCenter.y - radius);
        int maxY = Mathf.CeilToInt(localCenter.y + radius);

        int minZ = Mathf.FloorToInt(localCenter.z - radius);
        int maxZ = Mathf.CeilToInt(localCenter.z + radius);

        for (int x = minX; x <= maxX; x++)
        {
            if (x < 0 || x > m_width) continue;
            for (int y = minY; y <= maxY; y++)
            {
                if (y < 0 || y > m_height) continue;
                for (int z = minZ; z <= maxZ; z++)
                {
                    if (z < 0 || z > m_width) continue;

                    Vector3 point = new Vector3(x, y, z);
                    float dist = Vector3.Distance(point, localCenter);

                    if (dist <= radius)
                    {
                        modified = true;

                        float falloff = 1f - (dist / radius); // smooth falloff
                        float original = heights[x, y, z];
                        float change = intensity * falloff;
                        float newValue = Mathf.Clamp01(original - change);

                        // Apply change to terrain
                        heights[x, y, z] = newValue;

                        // Store the original value (the part that got removed)
                        returnHeights[x, y, z] = (newValue != 1) || (original != 1); // = newValue < 1 ? (newValue > 0 ? newValue : original) : original;
                        //Debug.Log(original);
                    }
                }
            }
        }

        if (!modified) return returnHeights;

        MarchCubes();
        return returnHeights;
    }
    public void DeformCylinder(Vector3 worldDeformPosition, float radius, float height, float intensity)
    {
        bool modified = false;  
        Vector3 localCenter = worldDeformPosition - worldPosition;

        int minX = Mathf.FloorToInt(localCenter.x - radius);
        int maxX = Mathf.CeilToInt(localCenter.x + radius);

        int minY = Mathf.FloorToInt(localCenter.y - height);
        int maxY = Mathf.CeilToInt(localCenter.y + height);

        int minZ = Mathf.FloorToInt(localCenter.z - radius);
        int maxZ = Mathf.CeilToInt(localCenter.z + radius);

        for (int x = minX; x <= maxX; x++)
        {
            if (x < 0 || x > m_width) continue;
            for (int y = minY; y <= maxY; y++)
            {
                if (y < 0 || y > m_height) continue;
                for (int z = minZ; z <= maxZ; z++)
                {
                    if (z < 0 || z > m_width) continue;

                    Vector3 point = new Vector3(x, localCenter.y, z);
                    float dist = Vector3.Distance(point, localCenter);

                    if (dist <= radius)
                    {
                        modified = true;
                        heights[x, y, z] = Mathf.Clamp01(heights[x, y, z] - intensity);
                    }
                }
            }
        }
        
        if (!modified) return;

        MarchCubes();
    }


    public void DeformCube(Vector3 worldDeformPosition, float width, float height, float length, float intensity)
    {
        bool modified = false;
        Vector3 localCenter = worldDeformPosition - worldPosition;

        int minX = Mathf.FloorToInt(localCenter.x - width);
        int maxX = Mathf.CeilToInt(localCenter.x + width);

        int minY = Mathf.FloorToInt(localCenter.y - height);
        int maxY = Mathf.CeilToInt(localCenter.y + height);

        int minZ = Mathf.FloorToInt(localCenter.z - length);
        int maxZ = Mathf.CeilToInt(localCenter.z + length);

        for (int x = minX; x <= maxX; x++)
        {
            if (x < 0 || x > m_width) continue;
            for (int y = minY; y <= maxY; y++)
            {
                if (y < 0 || y > m_height) continue;
                for (int z = minZ; z <= maxZ; z++)
                {
                    if (z < 0 || z > m_width) continue;

                    modified = true;
                    heights[x, y, z] = Mathf.Clamp01(heights[x, y, z] - intensity);
                }
            }
        }

        if(!modified) return;

        MarchCubes();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        if (!visualizeNoise || !Application.isPlaying)
        {
            return;
        }

        for (int x = 0; x < m_width + 1; x++)
        {
            for (int y = 0; y < m_height + 1; y++)
            {
                for (int z = 0; z < m_width + 1; z++)
                {
                    {
                        Gizmos.color = new Color(heights[x, y, z], heights[x, y, z], heights[x, y, z], 1);
                        Gizmos.DrawSphere(new Vector3(x, y, z), 0.2f);
                    }
                }
            }
        }
    }


}
