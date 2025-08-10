using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EndSceneSetup : MonoBehaviour
{
    [SerializeField] ScoreboardAppearScript appearScript;
    [SerializeField] List<GameObject> endSetups;

    [Header("WinText")]
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] string tieString;
    [SerializeField] string winString;
    private void Awake()
    {
        appearScript.Disapear();

        foreach (GameObject go in endSetups)
        {
            go.SetActive(false);
        }

        List<PlayerConfig> sortedConfigs = PlayerConfigManager.Instance.GetPlayerConfigs()
             .OrderBy(config => config.finalPoints)
             .ToList();

        sortedConfigs.Reverse();

        float currentHighest = -1;

        List<string> winnerNames = new List<string>();

        for (int i = 0; i < sortedConfigs.Count; i++)
        {
            if (i < endSetups.Count) {

                bool isWinner = false;
                if (currentHighest <= sortedConfigs[i].finalPoints)
                {
                    currentHighest = sortedConfigs[i].finalPoints;
                    isWinner = true;

                    winnerNames.Add("P" + (sortedConfigs[i].playerIndex + 1));
                }

                endSetups[i].GetComponent<EndPlayerVisualSetup>()?.SetPlayerConfig(sortedConfigs[i], isWinner);
                endSetups[i].SetActive(true);
            }
        }

        if (winnerNames.Count > 1) 
        {
            string tieResult = "";
            for (int i = 0;i < winnerNames.Count; i++)
            {
                tieResult += winnerNames[i] + (i == winnerNames.Count - 1 ? $" {tieString}" : ", ");
            }

            winText.text = tieResult;
        }
        else
        {
            winText.text = $"{winnerNames[0]} {winString}";
        }
    }
}
