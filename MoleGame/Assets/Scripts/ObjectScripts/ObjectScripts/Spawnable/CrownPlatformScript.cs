using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
public class CrownPlatformScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float popUpDuration;
    [SerializeField] float moveDownSpeed;
    [SerializeField] float holdDuration;
    [SerializeField] int wobbleCountBeforeDespawn;
    [SerializeField] float wobbleAmount;
    [SerializeField] float wobbleDuration;

    [SerializeField] BoxCollider trigger;
    [SerializeField] Transform objectHolder;
    Coroutine despawnRoutine = null;
    void Start()
    {
        trigger.enabled = false;
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, popUpDuration).OnComplete(() => trigger.enabled = true);
    }

    private IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(holdDuration);

        for (int i = 0; i < wobbleCountBeforeDespawn; i++)
        {
            yield return objectHolder.DOLocalMove((Vector3.forward + Vector3.left) * wobbleAmount * (i%2 == 0 ? 1 : - 1), wobbleDuration).SetEase(Ease.OutSine).WaitForCompletion();
        }

        while (true)
        {
            transform.position -= Vector3.up * moveDownSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(despawnRoutine != null)
            return;

        if(other.GetComponent<PlayerActionHandler>() != null)
        {
            despawnRoutine = StartCoroutine(DespawnRoutine());
        }   
    }
}
