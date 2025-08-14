using System;
using UnityEngine;

public class IPowerupScript : GrabbableComponent
{
    private bool poweredUp = false;
    internal virtual void Start()
    {
        isReleasable = false;
        ObjectManager.Instance.RegisterObject(gameObject);
    }

    public override void OnGrab(Transform _grabTransform, Action releaseFunc)
    {
        if (!poweredUp)
        {
            var target = _grabTransform.parent?.parent;

            if(target == null) { return; }


            DoPowerupEffect(target);

            if (target.GetComponent<PlayerActionHandler>() != null)
            {
                target.GetComponent<PlayerActionHandler>().OnGrab(false);
            }
            
            poweredUp = true;
        }
        releaseFunc();

        Destroy(gameObject);
    }

    internal virtual void DoPowerupEffect(Transform effectTransform)
    {

    }
}
