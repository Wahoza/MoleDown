using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerGrabTerrainComponent : MonoBehaviour
{
    [Header("grabSettings")]
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;
    [SerializeField] private float radiusIncrease;
    [SerializeField] private float heightThreshold = 0.5f;
    [SerializeField] private float intensity;
    private float radius;

    [Header("GrabObjects")]
    [SerializeField] GameObject grabResultPrefab;
    [SerializeField] Transform grabLocation;

    private int m_width;
    private int m_length;
    private int m_height;

    private float[,,] heights;
    private bool[,,] modHeights;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private int cubeCount;
    public async void GrabTerrain(float holdDuration)
    {
        radius = holdDuration * radiusIncrease;

        radius = radius < minRadius ? minRadius : radius > maxRadius ? maxRadius : radius;

        var grabResult = await LevelMarchingPlatform.instance.DeformedSphereReturnValues(grabLocation.position, radius, intensity);
        modHeights = grabResult;

        m_width = grabResult.GetLength(0);
        m_length = grabResult.GetLength(2);
        m_height = grabResult.GetLength(1);

        CreateTerrain(heights);
    }


    void CreateTerrain(float[,,] newHeights)
    {
        heights = new float[m_width, m_height, m_length];

        SetHeights();

        if (heights == null || heights.GetLength(0) == 0 || heights.GetLength(1) == 0 || heights.GetLength(2) == 0)
        {
            Debug.Log("Heights array is empty or null.");
            return;
        }

        //for (int x = 0; x < heights.GetLength(0); x++)
        //{
        //    for (int y = 0; y < heights.GetLength(1); y++)
        //    {
        //        for (int z = 0; z < heights.GetLength(2); z++)
        //        {
        //            float value = heights[x, y, z];
        //            Debug.Log($"heights[{x},{y},{z}] = {value}");
        //        }
        //    }
        //}
        MarchCubes();

        if (vertices.Count == 0 || triangles.Count == 0)
        {
            Debug.LogWarning("No geometry generated. Check thresholds or input data.");
            return;
        }

        var newObject = CreateObject();
        SetMesh(newObject);
    }
    void SetHeights()
    {
        cubeCount = 0;

        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_height; y++)
            {
                for (int z = 0; z < m_length; z++)
                {
                    float currentHeight = 0;
                    if (modHeights[x,y,z] == true) 
                    { 
                        currentHeight = m_height; 
                        cubeCount++;
                    }
                    else 
                    {
                        currentHeight = -1; 
                    }
                    float distToSurface;

                    if (y <= currentHeight - 0.5f)
                    {
                        distToSurface = 0f;
                    }
                    else if (y > currentHeight + 0.5f)
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

                    heights[x, y, z] = distToSurface; 
                } 
            } 
        }
    }
    private void MarchCubes()
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < m_width - 1; x++)
        {
            for (int y = 0; y < m_height - 1; y++)
            {
                for (int z = 0; z < m_length - 1; z++)
                {
                    float[] cubeCorneres = new float[8];


                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                        cubeCorneres[i] = heights[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), GetConfigurationIndex(cubeCorneres));
                }
            }
        }
    }
    private GameObject CreateObject()
    {
        var newObject = Instantiate(grabResultPrefab);

        GetComponent<PlayerGrabComponent>().GrabGrabbable(newObject.GetComponent<GrabbableComponent>());

        newObject.GetComponent<TerrainObjectComponent>()?.SetRigidbodyMassByCubeCount(cubeCount);
        newObject.GetComponent<TerrainObjectComponent>()?.SetRadius(radius);

        return newObject;
    }

    private void MarchCube(Vector3 position, int configIndex)
    {
        if (configIndex == 0 || configIndex == 255)
        {
            return;
        }

        int edgeIndex = 0;
        int lowestHeight = int.MaxValue;

        for (int y = 0; y < heights.GetLength(1); y++)
        {
            if (modHeights[m_width / 2, y, m_length / 2] && y < lowestHeight)
            {
                lowestHeight = y;
            }
        }

        if (lowestHeight == int.MaxValue)
        {
            lowestHeight = m_height / 2;
        }

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

                Vector3 vertex = (edgeStart + edgeEnd) / 2 - new Vector3(m_width, 0, m_length)/2 - Vector3.up * lowestHeight;

                vertices.Add(vertex);
                triangles.Add(vertices.Count - 1);

                edgeIndex++;
            }
        }
    }

    private int GetConfigurationIndex(float[] cubeCorners)
    {
        int configIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > heightThreshold)
            {
                configIndex |= 1 << i;
            }
        }

        return configIndex;
    }

    private void SetMesh(GameObject newObject)
    {
        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        newObject.GetComponent<MeshFilter>().mesh = mesh;
        newObject.GetComponent<MeshCollider>().sharedMesh = mesh;

        Debug.Log("finished Setting mesh");
    }
}
