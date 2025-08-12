using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(ObjectActionHandler))]
public class ObjectGrabbableComponent : GrabbableComponent
{
    [SerializeField] public Vector3 grabOffset;
    [SerializeField] private float moveToHoldTime = 0.1f;

    private IEnumerator grabUpdate;
    public Transform pocketTransform { get; private set; }
    public override void OnGrab(Transform _grabTransform, Action releaseFunc)
    {
        if (GetComponent<Rigidbody>() is var rb && rb != null)
        {
            rb.isKinematic = true;
        }

        base.OnGrab(_grabTransform, releaseFunc);

        if(grabUpdate == null)
        {
            grabUpdate = GrabUpdateCorutine();
        }

        pocketTransform = _grabTransform;

        transform.DOMove(_grabTransform.position, moveToHoldTime).SetEase(Ease.InQuad).OnComplete(() => StartCoroutine(grabUpdate));

        GetComponent<ObjectActionHandler>().OnGrab(true);
    }

    public override void OnRelease()
    {
        base.OnRelease();

        if (grabUpdate != null)
        {
            StopCoroutine(grabUpdate);
        }

        if (GetComponent<Rigidbody>() is var rb && rb != null)
        {
            rb.isKinematic = false;
        }

        transform.DOKill();

        pocketTransform = null;

        GetComponent<ObjectActionHandler>().OnGrab(false);
    }

    public override void OnThrow(Vector3 velocity)
    {
        Debug.Log("Throw");
        base.OnThrow(velocity);

        StopCoroutine(grabUpdate);

        if (GetComponent<Rigidbody>() is var rb && rb != null)
        {
            rb.isKinematic = false;

            rb.AddForce(velocity, ForceMode.Impulse);
        }

        GetComponent<ObjectActionHandler>().OnGrab(false);
    }

    private IEnumerator GrabUpdateCorutine()
    {
        while (true)
        {
            transform.position = pocketTransform.position + grabOffset;
            transform.rotation = pocketTransform.rotation;
            yield return null;
        }
    }

}
