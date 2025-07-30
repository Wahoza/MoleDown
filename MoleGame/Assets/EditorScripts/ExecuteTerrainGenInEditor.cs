using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExecuteTerrainGenInEditor : EditorWindow
{
    public MarchingWorld currentTerrainParent;
    private List<MarchingWorld> terrainParentsUsed = new();
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
                currentTerrainParent.Start();
            }
        }
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
