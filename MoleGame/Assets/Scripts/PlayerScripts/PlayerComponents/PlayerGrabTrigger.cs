using UnityEngine;

public class PlayerGrabTrigger : MonoBehaviour
{
    private PlayerGrabComponent grabComponent;
    
    public void SetPlayerGrabComponent(PlayerGrabComponent grabComponent)
    {
        this.grabComponent = grabComponent;
    }


    private void OnTriggerEnter(Collider other)
    {
        CheckColliderForGrab(other);
    }
    private void OnTriggerStay(Collider other)
    {
        CheckColliderForGrab(other);
    }
    void CheckColliderForGrab(Collider other)
    {
        if (other != transform.parent?.parent)
        {
            if (other.GetComponent<GrabbableComponent>() is GrabbableComponent grabbable && grabbable != null)
            {
                grabComponent.GrabGrabbable(grabbable);


                return;
            }
        }

        if (other.transform.parent != null)
        {
            if (other.transform.parent.GetComponent<GrabbableComponent>() is GrabbableComponent grabbable && grabbable != null)
            {
                grabComponent.GrabGrabbable(grabbable);
                return;
            }
        }
    }
}
