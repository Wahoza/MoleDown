using DG.Tweening;
using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardAppearScript : MonoBehaviour
{
    [SerializeField] float appearDuration;
    [SerializeField] Transform scoreHolder;
    [SerializeField] Image bgImage;
    public IEnumerator Appear()
    {
        bgImage.material.SetFloat("_FillAmount", 0);

        gameObject.SetActive(true);

        yield return DOTween.To(
        () => bgImage.material.GetFloat("_FillAmount"),
        x => bgImage.material.SetFloat("_FillAmount", x),
        1,
        appearDuration).SetUpdate(UpdateType.Normal, true).WaitForCompletion();
    }

    public IEnumerator Disapear()
    {
        bgImage.material.SetFloat("_FillAmount", 1);

        yield return DOTween.To(
        () => bgImage.material.GetFloat("_FillAmount"),
        x => bgImage.material.SetFloat("_FillAmount", x),
        0,
        appearDuration).SetUpdate(UpdateType.Normal, true).WaitForCompletion();

        gameObject.SetActive(false);
    }

    public Transform GetScoreHolder()
    {
        return scoreHolder;
    }
}
