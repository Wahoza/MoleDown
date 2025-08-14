using System;
using UnityEngine;

public class GrabbableComponent : MonoBehaviour
{
    public bool isGrabbed;
    public bool isReleasable = true;
    internal Transform grabTransform;
    internal Action releaseAction;
    public virtual void OnGrab(Transform _grabTransform, Action releaseFunc)
    {
        grabTransform = _grabTransform;
        releaseAction = releaseFunc;
        isGrabbed = true;
    }

    public virtual void OnRelease()
    {
        isGrabbed=false;
        grabTransform = null;
    }

    public virtual void OnThrow(Vector3 velocity)
    {
        OnRelease();

        grabTransform = null;
    }
}
