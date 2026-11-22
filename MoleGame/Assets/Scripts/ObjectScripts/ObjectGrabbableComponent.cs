using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(ObjectActionHandler))]
public class ObjectGrabbableComponent : GrabbableComponent
{
    [SerializeField] public Vector3 grabOffset;
    [SerializeField] public Vector3 grabRotation;

    [SerializeField] private float moveToHoldTime = 0.1f;
    [SerializeField] Transform overridenOwner;

    private IEnumerator grabUpdate;
    public Transform pocketTransform { get; private set; }

    private float grabOffsetLength = 0;

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

        transform.DOMove(pocketTransform.position + pocketTransform.TransformDirection(grabOffset).normalized * grabOffsetLength, moveToHoldTime).SetEase(Ease.InQuad).OnComplete(() => StartCoroutine(grabUpdate));
        transform.DORotate(_grabTransform.eulerAngles + grabRotation, moveToHoldTime);

        grabOffsetLength = grabOffset.magnitude;

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

        GetComponent<ObjectActionHandler>().OnGrab(false);

        pocketTransform = null;
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
            transform.position = pocketTransform.position + pocketTransform.TransformDirection(grabOffset).normalized * grabOffsetLength;
            transform.rotation = Quaternion.Euler(pocketTransform.rotation.eulerAngles + grabRotation);
            yield return null;
        }
    }

}
