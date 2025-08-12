using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class GameSettingsSelector : MonoBehaviour
{
    public static GameSettingsSelector Instance { get; private set; }

    [Header("Intro Anim")]
    [SerializeField] Vector3 startOffset;
    [SerializeField] float introDuration;
    [SerializeField] Transform holderTransform;
    [Header("SelectionThings")]
    [SerializeField] GameObject AToJoinText;
    [SerializeField] LoadingScreenComponent loadingComp;
    [SerializeField] TextMeshProUGUI selectRoundsText;
    [SerializeField] TextMeshProUGUI selectScoringSettingsText;
    [SerializeField] TextMeshProUGUI selectReloadSettingsText;
    [SerializeField] List<GameObject> selectionArrows;

    [Header("DefaultSettings")]
    [SerializeField] private int defaultRounds;
    [SerializeField] private int defaultScoring;
    [SerializeField] private string allScoreString;
    [SerializeField] private string winnerScoreString;
    [SerializeField] private int defaultReload;
    [SerializeField] private string generateNewString;
    [SerializeField] private string reloadOldString;
    [Header("Option Settings")]
    [SerializeField] private int maxRounds;
    [SerializeField] int optionCount;


    private PlayerConfig lastPlayer;
    private PlayerInputMap controller;

    private bool settingsStarted;

    private int currentSelectedField;

    private int currentRounds;
    private int currentScoring;
    private int currentReload;

    private float timeInPhase;
    private float invertedCurrentSpeed;
    private Vector2 lastDirectionInput;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;

            holderTransform.localPosition = startOffset;

            selectRoundsText.text = defaultRounds.ToString();
            currentRounds = defaultRounds;

            currentScoring = defaultScoring;

            string currentScoringSettings = "";

            switch (currentScoring)
            {
                case 0:
                    currentScoringSettings = winnerScoreString;
                    break;
                case 1:
                    currentScoringSettings = allScoreString;
                    break;
            }

            selectScoringSettingsText.text = currentScoringSettings;

            currentReload = defaultReload;
            string currentReloadSettings = "";

            switch (currentReload)
            {
                case 0:
                    currentReloadSettings = generateNewString;
                    break;
                case 1:
                    currentReloadSettings = reloadOldString;
                    break;
            }

            selectReloadSettingsText.text = currentReloadSettings;

            Debug.Log("Here");
        }
        else
        {
            Destroy(this);
        }
    }
    
    public void StartSettings()
    {
        AToJoinText?.SetActive(false);

        currentSelectedField = 0;
        foreach (var item in selectionArrows)
        {
            item.SetActive(false);
        }

        selectionArrows[0].SetActive(true);

        holderTransform.DOKill();
        holderTransform.localPosition = startOffset;
        holderTransform.DOLocalMove(Vector3.zero, introDuration).OnComplete(() => { settingsStarted = true; });

        SetPlayerInput(PlayerConfigManager.Instance.GetPlayerConfigs()[0]);
    }

    private void SetPlayerInput(PlayerConfig playerConfig)
    {
        if (playerConfig == lastPlayer) { return; }

        controller = new PlayerInputMap();

        playerConfig.Input.onActionTriggered += Input_onActionTriggered;
        if (lastPlayer != null)
        {
            lastPlayer.Input.onActionTriggered -= Input_onActionTriggered;
        }

        lastPlayer = playerConfig;
    }

    private void Input_onActionTriggered(CallbackContext obj)
    {
        if (!settingsStarted) { return; }

        if (obj.action.name == controller.UI.Navigate.name)
        {
            OnDirectionInput(obj.ReadValue<Vector2>());
        }
        if (obj.action.name == controller.UI.Submit.name)
        {
            Confirm();
        }
        if (obj.action.name == controller.UI.Cancel.name)
        {
            Cancel();
        }
    }
    private void OnDirectionInput(Vector2 direction)
    {
        if (!settingsStarted) { return; }
        direction.x = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? direction.x : 0;
        direction.y = Mathf.Abs(direction.y) >= Mathf.Abs(direction.x) ? direction.y : 0;


        direction.x = direction.x > 0 ? 1 : (direction.x < 0 ? -1 : 0);
        direction.y = direction.y > 0 ? -1 : (direction.y < 0 ? 1 : 0); //kinda weird but makes sense trust

        
        if (direction.y != 0 && direction.y != lastDirectionInput.y)
        {
            selectionArrows[currentSelectedField].SetActive(false);

            currentSelectedField += (int)direction.y;
            currentSelectedField = currentSelectedField < 0 ? optionCount : (currentSelectedField > optionCount ? 0 : currentSelectedField);

            selectionArrows[currentSelectedField].SetActive(true);

        }


        if (direction.x != 0 && direction.x != lastDirectionInput.x)
        {
            switch (currentSelectedField)
            {
                case 0:
                    Debug.Log("Here");
                    currentRounds += (int)(1 * direction.x);
                    currentRounds = currentRounds < 1 ? 1 : (currentRounds > maxRounds ? maxRounds : currentRounds);

                    selectRoundsText.text = currentRounds.ToString();

                    break;

                case 1:
                    currentScoring += (int)(1 * direction.x);
                    currentScoring = currentScoring < 0 ? 1 : (currentScoring > 1 ? 0 : currentScoring);

                    string currentScoringSettings = "";

                    switch (currentScoring)
                    {
                        case 0:
                            currentScoringSettings = winnerScoreString;
                            break;
                        case 1:
                            currentScoringSettings = allScoreString;
                            break;
                    }

                    selectScoringSettingsText.text = currentScoringSettings;
                    break;
                case 2:
                    currentReload += (int)(1 * direction.x);
                    currentReload = currentReload < 0 ? 1 : (currentReload > 1 ? 0 : currentReload);

                    string currentReloadSettings = "";

                    switch (currentReload)
                    {
                        case 0:
                            currentReloadSettings = generateNewString;
                            break;
                        case 1:
                            currentReloadSettings = reloadOldString;
                            break;
                    }

                    selectReloadSettingsText.text = currentReloadSettings;
                    break;
            }
        }

        lastDirectionInput = direction;
    }
    public void Cancel()
    {
        if(!settingsStarted) { return; }

        if (PlayerConfigManager.Instance != null)
        {
            PlayerConfigManager.Instance.UnReadyAll();
        }
        if (AToJoinText != null)
        {
            AToJoinText.SetActive(true);
        }
        //OptionField.SetActive(false);
        holderTransform.DOKill();
        holderTransform.DOLocalMove(startOffset, introDuration);
        settingsStarted = false;
    }
    private void Confirm()
    {
        if (currentSelectedField == optionCount)
        {
            settingsStarted = false;

            GameSettingsManager.Instance.SetRounds(currentRounds);
            GameSettingsManager.Instance.SetScoringSettings((GameSettingsManager.ScoringSettings)currentScoring);
            GameSettingsManager.Instance.SetReloadSettings((GameSettingsManager.LevelReloadSettings)currentReload);

            loadingComp.LoadScene(2);

            settingsStarted = false;

            //GameSettingsManager.Instance.SetPointsForPlacement(currentPoints);
            //GameSettingsManager.Instance.SetPointsPerSecond(currentTimePoints / currentTime);
            //GameSettingsManager.Instance.SetRequiredPoints(currentReqPoints);

            //if (FinishedSetupEvent != null)
            //{
            //    FinishedSetupEvent.Invoke(this, EventArgs.Empty);
            //}
            ////PlayerConfigManager.Instance.StartGame();
            ////OptionField.SetActive(false);
            //if (PlayerConfigManager.Instance != null)
            //{
            //    settingsAnim.SetTrigger("LoadGame");
            //}
            //else
            //{
            //    Cancel();
            //}
            //if (loadingScript != null)
            //{
            //    loadingScript.LoadScene(Loader.GetSceneID(SceneToLoad));
            //}
        }
    }
    //private void DoSettingsUpdate()
    //{
    //    timeInPhase += Time.deltaTime;
    //    if (timeInPhase > invertedCurrentSpeed)
    //    {
    //        switch (currentSelectedField)
    //        {
    //            case 0:
    //                currentRounds += (int)(1 * lastDirectionInput.x);
    //                currentRounds = currentRounds < 1 ? 1 : (currentRounds > maxRounds ? maxRounds : currentRounds);

    //                if (invertedCurrentSpeed > maxInvertedTimeIncrease && lastDirectionInput.x != 0)
    //                {
    //                    invertedCurrentSpeed *= invertedSpeedIncrease;
    //                }

    //                selectRoundsText.text = currentRounds.ToString();
                    
    //                return;

    //            case 1:
    //                currentScoring += (int)(1 * lastDirectionInput.x);
    //                currentScoring = currentScoring < 0 ? 1 : (currentScoring > 1 ? 0 : currentScoring);

    //                string currentScoringSettings = "";

    //                switch (currentScoring)
    //                {
    //                    case 0:
    //                        currentScoringSettings = winnerScoreString;
    //                        break;
    //                    case 1:
    //                        currentScoringSettings = allScoreString;
    //                        break;
    //                }

    //                selectScoringSettingsText.text = currentScoringSettings;
    //                return;
    //        }
    //    }
    //}
}
