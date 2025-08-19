using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance {get; private set;}

    
    public enum ScoringSettings {
        Winner = 0,
        All = 1
    }
    public enum LevelReloadSettings
    {
        GenerateNew = 0,
        ReloadOld = 1
    }
    private float insaneRoundChance;
    private int rounds;
    private ScoringSettings scoring;
    private LevelReloadSettings levelReload;
    public void SetScoringSettings(ScoringSettings settings)
    {
        scoring = settings;
    }
    public void SetRounds(int rounds)
    {
        this.rounds = rounds;
    }
    public void SetReloadSettings(LevelReloadSettings reload)
    {
        this.levelReload = reload;
    }

    public void SetInsaneRoundChance(float insaneRoundChance)
    {
        this.insaneRoundChance = insaneRoundChance;
    }
    public float GetInsaneRoundChance()
    {
        return insaneRoundChance;
    }
    public int GetRounds()
    {
        return rounds;
    }
    public ScoringSettings GetScoreSettings()
    {
        return scoring;
    }

    public LevelReloadSettings GetReloadSettings()
    {
        return levelReload;
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
