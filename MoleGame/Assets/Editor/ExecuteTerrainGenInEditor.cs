using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class ExecuteTerrainGenInEditor : EditorWindow
{
    public MarchingWorld currentTerrainParent;
    private List<MarchingWorld> terrainParentsUsed = new();
    string meshFilePath;
    [MenuItem("Tools/Generate Terrain Previs")]

    public static void ShowWindow()
    {
        GetWindow<ExecuteTerrainGenInEditor>("Generate Terrain Previs");
    }

    void OnGUI()
    {
        GUILayout.Label("Generate Terrain", EditorStyles.boldLabel);

        currentTerrainParent = (MarchingWorld)EditorGUILayout.ObjectField(
                    "Terrain Owner (MarchingWorld)",
                    currentTerrainParent,
                    typeof(MarchingWorld),
                    true); // allow scene objects

        

        if (GUILayout.Button("Generate Terrain Previs"))
        {
            if (currentTerrainParent != null)
            {
                for (int i = currentTerrainParent.transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(currentTerrainParent.transform.GetChild(i).gameObject);
                }

                terrainParentsUsed.Add(currentTerrainParent);
                currentTerrainParent.GenerateWorld();
            }
        }

        meshFilePath = GUILayout.TextField(meshFilePath);

        if (GUILayout.Button("Attempt Terrain Save"))
        {
            AttemptSaveTerrain();
        }

    }

    void AttemptSaveTerrain()
    {
        if(currentTerrainParent != null)
        {
            if(currentTerrainParent.transform.childCount > 0)
            {
                for (int i = 0; i < currentTerrainParent.transform.childCount; i++)
                {
                    var child = currentTerrainParent.transform.GetChild(i);

                    if (child.GetComponent<MeshFilter>() == null)
                        continue;

                    var mesh = child.GetComponent<MeshFilter>().sharedMesh;

                    string name = $"IslandElem{i}";

                    mesh.name = name;

                    SaveMesh(mesh, name, true, false);
                }
            }
        }
    }

    public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

        if (optimizeMesh)
            MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }
    private void OnDestroy()
    { 
        foreach (MarchingWorld terrainParent in terrainParentsUsed)
        {
            for (int i = terrainParent.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(terrainParent.transform.GetChild(i).gameObject);
            }
        }

        terrainParentsUsed.Clear();
    }
}
