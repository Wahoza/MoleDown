using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;
using DG.Tweening;

public class DestroyOnActionComponent : IActionComponent
{
    [SerializeField] float timeUntilDestruction;
    [SerializeField] float dissapearSpeed;

    private Coroutine destroyRoutine;
    
    bool dying = false;

    private void Awake()
    {
        GetComponent<ObjectActionHandler>().RegisterGrabAction(OnGrab);
    }
    private void OnDestroy()
    {
        GetComponent<ObjectActionHandler>().UnregisterGrabAction(OnGrab);
    }
    private void OnGrab(bool grab)
    {
        if (destroyRoutine != null)
        {
            StopCoroutine(destroyRoutine);
            transform.DOKill();
            destroyRoutine = null;
            dying = false;
        }
    }
    public override void DoAction()
    {
        if (dying) { return; }


        if (destroyRoutine == null && !(timeUntilDestruction == 0))
        {
            destroyRoutine = StartCoroutine(DestroyAfterTime());
        }
        else if(timeUntilDestruction <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        dying = true;

        yield return new WaitForSeconds(timeUntilDestruction);

        transform.DOScale(Vector3.zero, dissapearSpeed).SetEase(Ease.InSine).SetSpeedBased(true).OnComplete(() => Destroy(gameObject));
    }
}
