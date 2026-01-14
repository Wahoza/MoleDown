using DG.Tweening;
using System.Collections;
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

    [Header("Continue Move In Settigns")]
    [SerializeField] private float startHeight;
    [SerializeField] private float moveUpTime;
    [SerializeField] private Transform aToContinue;
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

    public IEnumerator StartContinuePrompt()
    {
        var pos = aToContinue.localPosition;
        pos.y = startHeight;
        aToContinue.localPosition = pos;

        aToContinue.gameObject.SetActive(true);

        aToContinue.DOKill();

        yield return aToContinue.DOLocalMoveY(0, moveUpTime).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal, true).WaitForCompletion();
    }
    public IEnumerator EndContinuePrompt()
    {
        var pos = aToContinue.localPosition;
        pos.y = 0;
        aToContinue.localPosition = pos;

        aToContinue.DOKill();
        yield return aToContinue.DOLocalMoveY(startHeight, moveUpTime).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal, true).WaitForCompletion();
        aToContinue.gameObject.SetActive(false);
    }

}
