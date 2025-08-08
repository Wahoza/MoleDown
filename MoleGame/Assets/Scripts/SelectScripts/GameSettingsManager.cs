using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance {get; private set;}

    public enum ScoringSettings {
        Winner = 0,
        All = 0
    }
    private int rounds;
    private ScoringSettings scoring;

    public void SetScoringSettings(ScoringSettings settings)
    {
        scoring = settings;
    }
    public void SetRounds(int rounds)
    {
        this.rounds = rounds;
    }

    public int GetRounds()
    {
        return rounds;
    }
    public ScoringSettings GetScoreSettings()
    {
        return scoring;
    }
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
