using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;
using DG.Tweening;

public class DestroyOnCollisionComponent : MonoBehaviour
{
    [SerializeField] float timeUntilDestruction;
    [SerializeField] float dissapearSpeed;
    [SerializeField] bool collisionWithKinematicRB;
    [SerializeField] LayerMask collisionLayerMask;

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
            destroyRoutine = null;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collisionLayerMask.value + " " + collision.collider.gameObject.layer);
        if(dying) { return; }


        if ((collisionLayerMask & (1 << collision.collider.gameObject.layer)) != 0)
        {
            if (collisionWithKinematicRB)
            {
                if(destroyRoutine == null)
                {
                    destroyRoutine = StartCoroutine(DestroyAfterTime());
                }
            }
            else if (GetComponent<Rigidbody>() is var rb && rb != null && !rb.isKinematic)
            {
                if (destroyRoutine == null)
                {
                    destroyRoutine = StartCoroutine(DestroyAfterTime());
                }
            }
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        dying = true;

        yield return new WaitForSeconds(timeUntilDestruction);

        transform.DOScale(Vector3.zero, dissapearSpeed).SetEase(Ease.InSine).SetSpeedBased(true).OnComplete(() => Destroy(gameObject));
    }
}
