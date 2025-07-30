using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerInputHandler), typeof(Rigidbody), typeof(PlayerStateComponent))]
public class PlayerMoveComponent : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float groundControll;
    [SerializeField] private float airControll;
    [SerializeField] private float maxUpVelocity;
    [SerializeField] private float maxUpAdjustmentSpeed;

    [Header("ForcedGrounding/Removal of upward Velocity")]
    [SerializeField] private bool forceGrounded = true;
    [SerializeField] private float maxGroundedYVelocity = 0.9f;

    [Header("Grounding Jump Disable")]
    [SerializeField] private float groundingDisableDuration;


    private Vector2 inputVector;
    private Vector3 moveDirection;

    private bool canMove = true;
    private bool isStunned = false;
    private bool isGrounded = true;

    private float stunTime;

    private Rigidbody rb;

    private PlayerStateComponent movementSettings;
    private PlayerActionHandler actionHandler;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movementSettings = GetComponent<PlayerStateComponent>();

        movementSettings.JoinComponent(this);

        GetComponent<PlayerInputHandler>().RegisterMoveFuncs(SetInputVector);

        actionHandler = GetComponent<PlayerActionHandler>();
        actionHandler.RegisterJumpActions(DisableGroundingForJump);

        StartCoroutine(ForceGround());
        StartCoroutine(UpdateGroundedState());
    }
    private IEnumerator ForceGround()
    {
        while (true)
        {
            if (forceGrounded && isGrounded && canMove)
            {
                var currentVelocity = rb.linearVelocity;
                if (currentVelocity.y > maxGroundedYVelocity)
                {
                    currentVelocity.y = maxGroundedYVelocity;
                }
                rb.linearVelocity = currentVelocity;
            }
            yield return null;
        }
    }
    private IEnumerator UpdateGroundedState()
    {
        while (true)
        {
            if (movementSettings.CanMove())
            {
                isGrounded = movementSettings.IsGrounded();
            }

            yield return null;
        }
    }

    private void MovementUpdate()
    {
        if (!movementSettings.CanMove()) { return; }
        //if (movementSettings.IsStunned())
        //{
        //    if (stunTime < Time.time)
        //    {
        //        transform.rotation = Quaternion.identity;
        //        movementSettings.SetStunned(false);
        //    }
        //    else
        //    {
        //        return;
        //    }
        //}

        moveDirection.Set(inputVector.x, 0, inputVector.y);

        moveDirection *= moveSpeed;
        Vector3 mixedVelocity = Vector3.Lerp(rb.linearVelocity, moveDirection, (isGrounded ? groundControll : airControll));

        if (isGrounded)
        {
            actionHandler.OnWalk(mixedVelocity);
        }

        if (maxUpVelocity < rb.linearVelocity.y && !isGrounded)
        {
            float upVelocity = Mathf.Lerp(rb.linearVelocity.y, maxUpVelocity, maxUpAdjustmentSpeed);
            rb.linearVelocity = new Vector3(mixedVelocity.x, upVelocity, mixedVelocity.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(mixedVelocity.x, rb.linearVelocity.y, mixedVelocity.z);
        }
    }

    public void StunPlayer(float stunDuration, Vector3 forceDirection)
    {
        movementSettings.SetStunned(true);

        stunTime = Time.time + stunDuration;

        if (forceDirection.sqrMagnitude > 0.001f)
        {
            forceDirection.Normalize();
            Quaternion lookRotation = Quaternion.LookRotation(forceDirection, Vector3.up);
            Quaternion additionalRotation = Quaternion.Euler(90f, 0f, 0f);
            transform.rotation = lookRotation * additionalRotation;
        }
    }

    void FixedUpdate()
    {
        if (!movementSettings.CanMove()) { return; }

        MovementUpdate();
    }

    public void SetInputVector(Vector2 direction)
    {
        inputVector = direction;
        if ((inputVector * 100f).sqrMagnitude > 10000)
        {
            inputVector.Normalize();
        }
    }

    private void DisableGroundingForJump()
    {
        StartCoroutine(OnJumpGroundDisableWait());
    }
    private IEnumerator OnJumpGroundDisableWait()
    {
        forceGrounded = false;

        yield return new WaitForSeconds(groundingDisableDuration);

        forceGrounded = true;

        yield return null;
    }
    private void OnDestroy()
    {
        GetComponent<PlayerInputHandler>().UnregisterInputMoveFuncs(SetInputVector);
        actionHandler.UnregisterJumpAction(DisableGroundingForJump);

    }
}
