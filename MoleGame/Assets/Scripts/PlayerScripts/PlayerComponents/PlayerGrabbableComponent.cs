using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerStateComponent), typeof(PlayerActionHandler))]
public class PlayerGrabbableComponent : GrabbableComponent
{
    [SerializeField] Vector3 grabOffset;
    [SerializeField] int jumpsRequiredForEscape;

    private Coroutine grabRoutine;
    private int jumpsSinceGrabbed = 0;


    private void Start()
    {
        GetComponent<PlayerInputHandler>().RegisterInputJumpFuncs((ctx) => IncrementJumpCtr());
        GetComponent<PlayerActionHandler>().RegisterDeathAction(OnDie);
        GetComponent<PlayerActionHandler>().RegisterSpawnAction(OnSpawn);
    }
    private void OnSpawn()
    {
        OnRelease();
    }
    public override void OnGrab(Transform _grabTransform, Action releaseFunc)
    {
        jumpsSinceGrabbed = 0;

        GetComponent<PlayerStateComponent>().SetGrabbed(true);
        GetComponent<PlayerActionHandler>().OnGrabbable(true);



        if (GetComponent<Rigidbody>() is Rigidbody rb && rb != null)
        {
            rb.isKinematic = true;
        }

        base.OnGrab(_grabTransform, releaseFunc);

        grabRoutine = StartCoroutine(GrabUpdateCorutine());
    }

    public override void OnRelease()
    {
        GetComponent<PlayerStateComponent>().SetGrabbed(false);
        GetComponent<PlayerActionHandler>().OnGrabbable(false);

        if (GetComponent<Rigidbody>() is Rigidbody rb && rb != null)
        {
            rb.isKinematic = false;
        }

        if (grabRoutine != null)
        {
            StopCoroutine(grabRoutine);
        }
        base.OnRelease();
    }

    public override void OnThrow(Vector3 velocity)
    {
        OnRelease();

        if (GetComponent<Rigidbody>() is Rigidbody rb && rb != null)
        {
            rb.AddForce(velocity, ForceMode.Impulse);
        }
        base.OnThrow(velocity);
    }

    void OnDie()
    {
        OnRelease();
    }
    private void IncrementJumpCtr()
    {
        jumpsSinceGrabbed++;
        Debug.Log("JUMPPP" + jumpsSinceGrabbed);
    }
    private IEnumerator GrabUpdateCorutine()
    {
        while (true)
        {
            transform.position = grabTransform.position + grabOffset;
            transform.rotation = grabTransform.rotation;

            if (jumpsSinceGrabbed >= jumpsRequiredForEscape)
            {
                releaseAction(); //force release
            }

            yield return null;
        }
    }

    private void OnDestroy()
    {
        GetComponent<PlayerActionHandler>().GetComponent<PlayerInputHandler>().UnregisterInputJumpFuncs((ctx) => IncrementJumpCtr());
        GetComponent<PlayerActionHandler>().UnregisterDeathAction(OnDie);
        GetComponent<PlayerActionHandler>().UnregisterSpawnAction(OnSpawn);
    }
}
