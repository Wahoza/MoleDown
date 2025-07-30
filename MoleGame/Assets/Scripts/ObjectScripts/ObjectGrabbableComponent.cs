using System;
using System.Collections;
using UnityEngine;

public class ObjectGrabbableComponent : GrabbableComponent
{
    [SerializeField] public Vector3 grabOffset;

    private IEnumerator grabUpdate;
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

        StartCoroutine(grabUpdate);
    }

    public override void OnRelease()
    {
        base.OnRelease();

        StopCoroutine(grabUpdate);

        if (GetComponent<Rigidbody>() is var rb && rb != null)
        {
            rb.isKinematic = false;
        }
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
    }

    private IEnumerator GrabUpdateCorutine()
    {
        while (true)
        {
            transform.position = grabTransform.position + grabOffset;
            transform.rotation = grabTransform.rotation;
            yield return null;
        }
    }

}
