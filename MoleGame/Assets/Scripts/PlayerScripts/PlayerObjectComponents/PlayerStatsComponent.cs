using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsComponent : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] RawImage moleImage;

    private PlayerConfig config;
    public void Initialize(PlayerConfig pc)
    {
        scoreText.color = pc.playerColor;
        moleImage.color = pc.playerColor;

        scoreText.text = "x" + pc.finalPoints.ToString();

        config = pc;

        if (GameSettingsManager.Instance.GetGameModeSettings() == GameSettingsManager.GameMode.Crown)
        {
            StartCoroutine(StatUpdateRoutine());
        }
    }

    private IEnumerator StatUpdateRoutine()
    {
        while (true)
        {
            UpdateStats();
            yield return null;
        }
    }
    
    
    public void UpdateStats()
    {
        if (config != null)
        {
            scoreText.text = "x" + (config.finalPoints + (int)config.pointsThisRound).ToString();
        }
    }
}
