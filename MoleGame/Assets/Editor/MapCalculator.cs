using UnityEngine;
using UnityEditor;
using System.IO;

public class DistanceFieldGenerator : EditorWindow
{
    public Texture2D inputTexture;
    public bool targetWhite = true;
    public bool invertResult = false;
    public string savePath = "Assets/Heightmaps/Maps/";
    public string mapName = "Default";
    public float falloffPower = 2;

    //[MenuItem("Tools/Generate Distance Field")]
    public static void ShowWindow()
    {
        GetWindow<DistanceFieldGenerator>("Distance Field Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Input Settings", EditorStyles.boldLabel);
        inputTexture = (Texture2D)EditorGUILayout.ObjectField("Input Texture", inputTexture, typeof(Texture2D), false);
        targetWhite = EditorGUILayout.Toggle("Target White Pixels", targetWhite);
        invertResult = EditorGUILayout.Toggle("Invert Image Result", invertResult);
        falloffPower = EditorGUILayout.FloatField("falloff Power of Image", falloffPower);
        savePath = EditorGUILayout.TextField("Save Path", savePath);
        mapName = EditorGUILayout.TextField("Map Name", mapName);

        if (GUILayout.Button("Generate Distance Field"))
        {
            if (inputTexture != null)
            {
                GenerateDistanceField();
            }
            else
            {
                Debug.LogError("Please assign an input texture.");
            }
        }
    }

    void GenerateDistanceField()
    {
        int width = inputTexture.width;
        int height = inputTexture.height;

        Texture2D output = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Preload pixels
        Color[] pixels = inputTexture.GetPixels();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float minDistSqr = float.MaxValue;

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        float gray = pixels[j * width + i].grayscale;
                        bool isMatch = targetWhite ? gray > 0.5f : gray <= 0.5f;

                        if (isMatch)
                        {
                            float dx = x - i;
                            float dy = y - j;
                            float distSqr = dx * dx + dy * dy;

                            if (distSqr < minDistSqr)
                                minDistSqr = distSqr;
                        }
                    }
                }

                float dist = Mathf.Sqrt(minDistSqr);
                float normalized = Mathf.Clamp01(dist / Mathf.Max(width, height)); // normalize distance
                float curved = Mathf.Pow(normalized, falloffPower);

                Color distColor = new Color(curved, curved, curved, 1f);

                if (invertResult)
                {
                    distColor = Color.white - distColor;
                }

                output.SetPixel(x, y, distColor);
            }
        }

        output.Apply();

        // Save to disk
        byte[] bytes = output.EncodeToPNG();
        string finalPath = savePath + mapName + ".png";
        File.WriteAllBytes(finalPath, bytes);
        AssetDatabase.Refresh();

        Debug.Log("Distance field generated and saved to: " + savePath);

        TextureImporter importer = AssetImporter.GetAtPath(finalPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.isReadable = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            Debug.Log("Marked texture as readable: " + savePath);
        }
    }
}