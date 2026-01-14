using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndPlayerVisualSetup : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject HandL;
    [SerializeField] GameObject HandR;
    [SerializeField] GameObject FootL;
    [SerializeField] GameObject FootR;
    [SerializeField] GameObject Body;
    [SerializeField] GameObject Eyes;
    [SerializeField] GameObject ModelHolder;
    [SerializeField] List<GameObject> winObjects;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI pointText;
    [SerializeField] TextMeshProUGUI nameText;
    public void SetPlayerConfig(PlayerConfig playerConfig, bool winner)
    {
        var bodyRenderer = Body.GetComponent<MeshRenderer>();
        Material[] mats = bodyRenderer.materials;

        // Clone the first material
        var mat = new Material(mats[0]);
        mat.color = playerConfig.playerColor;
        mats[0] = mat;

        Debug.Log("Setting new material");

        // Apply updated materials array to each mesh
        Body.GetComponent<MeshRenderer>().materials = mats;
        Eyes.GetComponent<MeshRenderer>().materials = mats;
        HandL.GetComponent<MeshRenderer>().materials = mats;
        HandR.GetComponent<MeshRenderer>().materials = mats;
        FootL.GetComponent<MeshRenderer>().materials = mats;
        FootR.GetComponent<MeshRenderer>().materials = mats;

        if (winner)
        {
            foreach (var go in winObjects)
            {
                go.SetActive(true);
            }
        }

        pointText.text = $"{playerConfig.finalPoints}";

        nameText.text = $"P{playerConfig.playerIndex + 1}";
        nameText.color = playerConfig.playerColor;
    }
}
