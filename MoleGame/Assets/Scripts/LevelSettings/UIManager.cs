using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] public ScoreboardAppearScript scoreBoardHolder;
    [SerializeField] public Transform pointerHolder;
    [SerializeField] public Transform playerStatsHolder;
    [SerializeField] private TextMeshProUGUI currentRoundText;
    [SerializeField] string currentRoundString;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentRound(int currentRound, int maxRound)
    {
        currentRoundText.text = $"{currentRoundString}: {currentRound}/{maxRound}";
    }
}
