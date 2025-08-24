using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DeerScript : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveDist;
    [SerializeField] float moveSpeed;
    [SerializeField] float randomRotOffset;
    [Header("Animation")]
    [SerializeField] float moveUpDuration;
    [SerializeField] float defaultY;
    [SerializeField] float startY;
    [SerializeField] Transform model;


    private IEnumerator Start()
    {
        Vector3 lookAt = GameManager.Instance.worldCenter.position;
        lookAt.y = transform.position.y; 

        Vector3 dir = (lookAt - transform.position).normalized;

        float angleOffset = Random.Range(-randomRotOffset, randomRotOffset);
        Quaternion rotation = Quaternion.Euler(0f, angleOffset, 0f);
        dir = rotation * dir;

        lookAt = transform.position + dir;

        transform.LookAt(lookAt);

        transform.position = transform.position + Vector3.up * startY;

        yield return transform.DOMoveY(defaultY, moveUpDuration).SetEase(Ease.OutCirc).WaitForCompletion();

        Vector3 destination = transform.forward * moveDist + transform.position;

        yield return transform.DOMove(destination, moveSpeed).SetSpeedBased(true).WaitForCompletion();

        yield return transform.DOMoveY(startY, moveUpDuration).SetEase(Ease.InCirc).WaitForCompletion();

        Destroy(gameObject);
    }
}
