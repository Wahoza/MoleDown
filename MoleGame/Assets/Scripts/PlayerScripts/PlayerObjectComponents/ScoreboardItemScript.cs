using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardItemScript : MonoBehaviour
{
    [SerializeField] RawImage moleImage;

    [SerializeField] TextMeshProUGUI playerIDText;
    [SerializeField] TextMeshProUGUI playerPointText;
    [SerializeField] TextMeshProUGUI playerPlacementText;
    [SerializeField] TextMeshProUGUI playerPointsThisRoundText;

    [SerializeField] Image backgroundImage;
    [SerializeField] Image backgroundPointsImage;

    [SerializeField] List<Texture> moleFaces;
    [SerializeField] List<string> placementStrings;

    [Header("Animation")]
    [SerializeField] float yOffset;
    [SerializeField] float animDuration;
    [SerializeField] Transform transformToAnimate;

    private PlayerConfig config;

    public void Initialize(PlayerConfig playerConfig)
    {
        backgroundImage.color = playerConfig.playerColor;
        backgroundPointsImage.color = playerConfig.playerColor;

        playerIDText.text = "P" + playerConfig.playerIndex;
        moleImage.color = playerConfig.playerColor;

        config = playerConfig;
    }
    public void SetPlacement(int placement)
    {
        int faceIndex = placement;
        if (faceIndex > moleFaces.Count - 1)
            faceIndex = moleFaces.Count - 1;

        moleImage.texture = moleFaces[faceIndex];
        moleImage.color = config.playerColor;

        int placementStringIndex = placement;
        if (placementStringIndex > placementStrings.Count - 1)
            placementStringIndex = placementStrings.Count - 1;

        playerPlacementText.text = (placement + 1) + placementStrings[placementStringIndex];
    }

    public void UpdatePoints()
    {
        playerPointText.text = config.finalPoints.ToString();
    }

    public void SetRoundPoints(int points)
    {
        playerPointsThisRoundText.text = "+" + points;
    }
    public void SetHeight()
    {
        transformToAnimate.localPosition = new Vector3(transform.localPosition.x, yOffset, transform.localPosition.z);
    }

    public IEnumerator DoFall()
    {
        yield return transformToAnimate.DOLocalMoveY(0, animDuration).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal, true).WaitForCompletion();
    }

    public IEnumerator DoRise()
    {
        yield return transformToAnimate.DOLocalMoveY(yOffset, animDuration).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal, true).WaitForCompletion();
    }
}
