using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerInputHandler), typeof(Rigidbody), typeof(PlayerStateComponent))]
public class PlayerMoveComponent : IPlayerComponent
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

    internal float currentMoveSpeed;
    internal float currentgroundControll;
    internal float currentairControll;
    internal float currentMaxUpVelocity;
    internal float currentMaxUpAdjustmentSpeed;

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
        actionHandler.RegisterJumpAction(DisableGroundingForJump);
        actionHandler.RegisterSpawnAction(OnSpawn);

        StartCoroutine(ForceGround());
        StartCoroutine(UpdateGroundedState());
    }
    internal override void ResetStatsOnRespawn()
    {
        currentMoveSpeed = moveSpeed;
        currentgroundControll       = groundControll;
        currentairControll          = airControll;
        currentMaxUpVelocity        = maxUpVelocity;
        currentMaxUpAdjustmentSpeed = maxUpAdjustmentSpeed;
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
                if(!rb.isKinematic) {
                    rb.linearVelocity = currentVelocity;
                }
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

    private void OnSpawn()
    {
        rb.linearVelocity = Vector3.zero;
        inputVector = Vector2.zero;
        moveDirection = Vector3.zero;
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
        if(rb.isKinematic) { return; }

        moveDirection.Set(inputVector.x, 0, inputVector.y);

        moveDirection *= currentMoveSpeed;
        Vector3 mixedVelocity = Vector3.Lerp(rb.linearVelocity, moveDirection, (isGrounded ? currentgroundControll : currentairControll) * Time.deltaTime);

        if (!isStunned)
        {
            actionHandler.OnWalk(mixedVelocity);
        }

        if (currentMaxUpVelocity < rb.linearVelocity.y && !isGrounded)
        {
            float upVelocity = Mathf.Lerp(rb.linearVelocity.y, currentMaxUpVelocity, currentMaxUpAdjustmentSpeed);
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
    internal override void OnDestroy()
    {
        base.OnDestroy();
        GetComponent<PlayerInputHandler>().UnregisterInputMoveFuncs(SetInputVector);
        actionHandler.UnregisterJumpAction(DisableGroundingForJump);
        actionHandler.UnregisterSpawnAction(OnSpawn);
    }
}
