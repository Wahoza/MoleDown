using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerGrabComponent : MonoBehaviour
{
    [Header("Grab Settings")]
    [SerializeField] float grabObjectAttemptDuration;
    [SerializeField] float grabGroundWaitBuffer;
    [SerializeField] float grabGroundMaxDuration;
    [SerializeField] Vector3 throwVelocity;
    [SerializeField] float throwForce;

    [Header("Grab Objects")]
    [SerializeField] PlayerGrabTrigger grabTrigger;
    [SerializeField] Transform pocketTransform;

    private GrabbableComponent grabbedGrabbable;
    private IEnumerator grabCoroutine;
    private float grabStartTime;
    private bool grabCanceled = true;
    private void Start()
    {
        GetComponent<PlayerInputHandler>().RegisterGrabFuncs(OnGrabInput);
        GetComponent<PlayerInputHandler>().RegisterInputAttackFuncs((ctx) => ThrowGrabbable());

        GetComponent<PlayerActionHandler>().RegisterJumpActions(CancelGrabAttemptAction);
        GetComponent<PlayerActionHandler>().RegisterGrabbableActions(CancelGrabAttemptGrab);

        grabCoroutine = GrabDisableWaitCoroutine();
        grabTrigger.SetPlayerGrabComponent(this);
    }

    void OnGrabInput(InputAction.CallbackContext ctx)
    {
        if(GetComponent<PlayerStateComponent>().IsGrabbed()) 
            return;


        if (ctx.phase == InputActionPhase.Started)
        {
            if (grabbedGrabbable != null)
            {
                ReleaseGrabbable();

            }
            else
            {
                grabTrigger.gameObject.SetActive(true);

                StartCoroutine(grabCoroutine);

                grabStartTime = Time.time;

                grabCanceled = false;

                GetComponent<PlayerActionHandler>().OnGrab(true);
            }

            Debug.Log("StartedGrab");
        }
        else if (ctx.phase == InputActionPhase.Canceled  || ctx.phase == InputActionPhase.Disabled)
        {
            grabTrigger.gameObject.SetActive(false);


            if (!grabCanceled)
            {
                GetComponent<PlayerActionHandler>().OnGrab(false);

                StopCoroutine(grabCoroutine);

                TryGrabGround();

                Debug.Log("Ended Grab");
            }
        }
    }

    void CancelGrabAttemptAction()
    {
        StopCoroutine(grabCoroutine);

        grabTrigger.gameObject.SetActive(false);

        grabCanceled = true;

    }
    void CancelGrabAttemptGrab(bool grabbed)
    {
        if (grabbed)
        {
            CancelGrabAttemptAction();

            GetComponent<PlayerActionHandler>().OnGrab(false);
        }
    }
    public void GrabGrabbable(GrabbableComponent grabbable)
    {
        GetComponent<PlayerActionHandler>().OnGrab(false);

        CancelGrabAttemptAction();

        Debug.Log("Grabbed Object");
        grabbable.OnGrab(pocketTransform, ReleaseGrabbable);

        grabbedGrabbable = grabbable;

        GetComponent<PlayerActionHandler>().OnGrab(true);
    }

    public void ReleaseGrabbable()
    {
        if (grabbedGrabbable != null)
        {
            Debug.Log("Released");

            grabbedGrabbable.OnRelease();
            grabbedGrabbable = null;


            GetComponent<PlayerActionHandler>().OnGrab(false);
        }
    }
    private IEnumerator GrabDisableWaitCoroutine()
    {
        yield return new WaitForSeconds(grabObjectAttemptDuration);

        grabTrigger.gameObject.SetActive(false);
    }

    bool TryGrabGround() {

        float elapsedGrabTime = Time.time - grabStartTime - grabObjectAttemptDuration - grabGroundWaitBuffer;

        Debug.Log("GroundGrabAttempt " + elapsedGrabTime.ToString());

        if (elapsedGrabTime > 0) {


            GetComponent<PlayerGrabTerrainComponent>()?.GrabTerrain(elapsedGrabTime);
        }

        return false;
    }

    void ThrowGrabbable()
    {
        if (grabbedGrabbable != null)
        {
            grabbedGrabbable.OnThrow((pocketTransform.forward * throwVelocity.x + pocketTransform.up * throwVelocity.y + pocketTransform.right * throwVelocity.z).normalized * throwForce);
            grabbedGrabbable = null;

            GetComponent<PlayerActionHandler>().OnGrab(false);
        }
    }
    private void OnDestroy()
    {
        GetComponent<PlayerInputHandler>().UnregisterInputGrabFuncs(OnGrabInput);
        GetComponent<PlayerInputHandler>().UnregisterInputAttackFuncs((ctx) => ThrowGrabbable());

        GetComponent<PlayerActionHandler>().UnregisterJumpAction(CancelGrabAttemptAction);
        GetComponent<PlayerActionHandler>().RegisterGrabbableActions(CancelGrabAttemptGrab);

    }
}
