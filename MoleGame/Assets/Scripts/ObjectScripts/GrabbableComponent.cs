using System;
using UnityEngine;

public class GrabbableComponent : MonoBehaviour
{
    internal Transform grabTransform;
    internal Action releaseAction;
    public virtual void OnGrab(Transform _grabTransform, Action releaseFunc)
    {
        grabTransform = _grabTransform;
        releaseAction = releaseFunc;
    }

    public virtual void OnRelease()
    {
        grabTransform = null;
    }

    public virtual void OnThrow(Vector3 velocity)
    {
        OnRelease();

        grabTransform = null;
    }
}
