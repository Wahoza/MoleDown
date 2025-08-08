using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownComponent : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textBox;
    [SerializeField] Vector3 startRotation;
    [SerializeField] Vector3 startScale;
    [SerializeField] float numberShowSpeed;
    [SerializeField] float fadeOutDuration;
    [SerializeField] string startText;
    private Coroutine countDownCoroutine;
    public void StartCountdown(int startValue, int endValue, float numberShowTime)
    {
        gameObject.SetActive(true);
        if (countDownCoroutine == null)
        {
            countDownCoroutine = StartCoroutine(CountdownCoroutine(startValue, endValue, numberShowTime));
        }
    }
   
    private IEnumerator CountdownCoroutine(int startValue, int endValue, float numberShowTime)
    {
        float adjustedShowTime = numberShowSpeed < numberShowTime ? numberShowSpeed : numberShowTime;
        textBox.text = startValue.ToString();

        yield return new WaitForSecondsRealtime(numberShowTime);

        for (int i = startValue - 1; i > endValue - 1; i--)
        {
            if (!GameManager.Instance.isPaused)
            {
                textBox.color = Color.white;
                if (i > endValue)
                {
                    textBox.text = i.ToString();
                }
                else
                {
                    textBox.text = startText;
                }

                transform.localScale = startScale;
                transform.localRotation = Quaternion.Euler(startRotation);
                transform.DOScale(Vector3.one, adjustedShowTime).SetUpdate(true);
                transform.DOLocalRotate(Vector3.zero, adjustedShowTime).SetUpdate(true);


                yield return new WaitForSecondsRealtime(numberShowTime);
                yield return textBox.DOColor(new Color(1, 1, 1, 0), fadeOutDuration).SetUpdate(true).WaitForCompletion();
            }
            else
            {
                while (GameManager.Instance.isPaused)
                {
                    yield return null;
                }
            }
        }

        gameObject.SetActive(false);
        countDownCoroutine = null;
        Time.timeScale = 1.0f;
    }

}
