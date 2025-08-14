using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler), typeof(Rigidbody), typeof(PlayerStateComponent))]
public class PlayerJumpComponent : IPlayerComponent
{
    [SerializeField] float jumpInputBuffer;
    [SerializeField] float jumpStrength;
    [SerializeField] float maxJumpVelocity;
    [SerializeField] float bufferedJumpInputDuration;

    private Rigidbody rb;
    private PlayerStateComponent movementSettings;
    private PlayerActionHandler actionHandler;


    private float jumpTimer;
    private Coroutine jumpBufferCoroutine;

    internal float currentJumpInputBuffer;
    internal float currentJumpStrength;
    internal float currentMaxJumpVelocity;
    internal float currentBufferedJumpInputDuration;

    internal override void Start()
    {
        GetComponent<PlayerInputHandler>().RegisterInputJumpFuncs(ctx => TryJump());

        movementSettings = GetComponent<PlayerStateComponent>();
        actionHandler = GetComponent<PlayerActionHandler>();

        rb = GetComponent<Rigidbody>();

        StartCoroutine(JumpVelocityReduction());

        base.Start();
    }
    internal override void ResetStatsOnRespawn()
    {
        base.ResetStatsOnRespawn();

        currentJumpInputBuffer = jumpInputBuffer;
        currentJumpStrength                = jumpStrength;
        currentMaxJumpVelocity             = maxJumpVelocity;
        currentBufferedJumpInputDuration   = bufferedJumpInputDuration;
    }
    public void TryJump()
    {
        if (movementSettings.IsGrounded() && Time.time > jumpTimer && movementSettings.CanMove() && Time.timeScale != 0)
        {
            Jump();
        }else{
            if (jumpBufferCoroutine != null)
            {
                StopCoroutine(jumpBufferCoroutine);
            }

            jumpBufferCoroutine = StartCoroutine(JumpInputBufferUpdate());
        }
    }
    private void Jump()
    {
        jumpTimer = Time.time + currentJumpInputBuffer;

        Vector3 adjustedVelocity = rb.linearVelocity;
        adjustedVelocity.y = 0;
        rb.linearVelocity = adjustedVelocity;

        rb.AddForce(Vector3.up * currentJumpStrength, ForceMode.Impulse);

        actionHandler.OnJump();
    }
    public void ForceJump()
    {
        Jump();
    }
    public IEnumerator JumpInputBufferUpdate()
    {
        float elapsed = 0f;
        while (elapsed < bufferedJumpInputDuration)
        {
            TryBufferedJump();
            elapsed += Time.deltaTime;
            yield return null;
        }

        jumpBufferCoroutine = null;
    }
    void TryBufferedJump()
    {
        if (movementSettings.IsGrounded() && Time.time > jumpTimer && movementSettings.CanMove() && Time.timeScale != 0)
        {
            Jump();

            if (jumpBufferCoroutine != null)
            {
                StopCoroutine(jumpBufferCoroutine);
            }
        }
    }

    private IEnumerator JumpVelocityReduction()
    {
        while (true)
        {
            if (!movementSettings.IsGrounded() && rb.linearVelocity.y > currentMaxJumpVelocity)
            {
                var velocity = rb.linearVelocity;
                velocity.y = currentMaxJumpVelocity;
                rb.linearVelocity = velocity;   
            }
            yield return null;
        }
    }
    internal override void OnDestroy()
    {
        base.OnDestroy();
        GetComponent<PlayerInputHandler>().UnregisterInputJumpFuncs(ctx => TryJump());
    }
}
