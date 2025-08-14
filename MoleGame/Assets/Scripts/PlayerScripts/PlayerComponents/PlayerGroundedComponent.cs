using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlayerStateComponent), typeof(PlayerActionHandler))]
public class PlayerGroundedComponent : IPlayerComponent
{
    [Header("Setup")]
    [SerializeField] BoxCollider FeetBox;
    [SerializeField] int alreadyExistingCols; // note this is for collisions with our character
    [SerializeField] LayerMask canStandLayers;

    [Header("Function Related")]
    [SerializeField] float landBufferTime;

    private bool isGrounded = true;
    private bool lastIsGrounded = true;
    private bool groundedDisabled = false;
    private float lastWasGroundedTime;
    private bool isGrabbed;
    private Coroutine disableGroundedRoutine;

    internal override void Start()
    {
        GetComponent<PlayerStateComponent>().JoinComponent(this);
        GetComponent<PlayerActionHandler>().RegisterGrabbableAction(OnGrab);
        GetComponent<PlayerActionHandler>().RegisterSpawnAction(OnRespawn);

        base.Start();
    }
    public void DisableGroundedForDuration(float duration)
    {
        if (disableGroundedRoutine != null)
        {
            StopCoroutine(disableGroundedRoutine);
        }

        disableGroundedRoutine = StartCoroutine(disableGrounded(duration));
    }
    private IEnumerator disableGrounded(float duration)
    {
        groundedDisabled = true;
        yield return new WaitForSeconds(duration);
        groundedDisabled = false;
        disableGroundedRoutine = null;
    }
    public bool GetGrounded()
    {
        return isGrounded;
    }

    void OnRespawn()
    {
        lastIsGrounded = true;
        isGrounded = true;
    }
    private void FixedUpdate()
    {
        if(FeetBox == null) return;

        isGrounded = false;
        if(groundedDisabled) { return; }

        var colliders = Physics.OverlapBox(FeetBox.transform.position, FeetBox.size / 2 * transform.localScale.x, transform.rotation, canStandLayers);

        if (colliders.Count() > alreadyExistingCols)
        {
            isGrounded = true;
        }

        CalculateLanding();
    }

    private void CalculateLanding()
    {
        if (!isGrounded && lastIsGrounded)
        {
            lastWasGroundedTime = Time.time;
        }

        if (!lastIsGrounded && isGrounded)
        {
            if (lastWasGroundedTime <= Time.time - landBufferTime) {

                if (!isGrabbed)
                {
                    Debug.Log("Landing!");
                    GetComponent<PlayerActionHandler>().OnLand(Time.time - lastWasGroundedTime);
                }
            }
        }

        lastIsGrounded = isGrounded;
    }
    void OnGrab(bool grabbed)
    {
        isGrabbed = grabbed;
        lastWasGroundedTime = Time.time;
    }

    internal override void OnDestroy()
    {
        base.OnDestroy();

        GetComponent<PlayerActionHandler>().UnregisterGrabbableAction(OnGrab);
        GetComponent<PlayerActionHandler>().UnregisterSpawnAction(OnRespawn);
    }
}
