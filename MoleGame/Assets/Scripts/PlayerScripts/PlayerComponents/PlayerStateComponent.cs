using System;
using UnityEngine;

public class PlayerStateComponent : MonoBehaviour
{
    [SerializeField] private bool isStunned;
    [SerializeField] private bool isCanMove;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isGrabbed;

    private PlayerGroundedComponent groundedComponent;
    private PlayerJumpComponent jumpComponent;
    private PlayerMoveComponent moveComponent;
    private PlayerGrabbableComponent grabbableComponent;
    public void JoinComponent(MonoBehaviour component)
    {
        if (component is PlayerGroundedComponent pgc)
        {
            groundedComponent = pgc;
        }

        if(component is PlayerJumpComponent pjc)
        {
            jumpComponent = pjc;
        }

        if(component is PlayerMoveComponent pmc)
        {
            moveComponent = pmc;
        }
    }
    public bool IsGrounded()
    {
        if(groundedComponent != null)
        {
            return groundedComponent.GetGrounded();
        }
        return true;
    }

    public bool CanMove()
    {
        return isCanMove;
    }

    public bool IsStunned()
    {
        return isStunned;
    }
    public bool IsGrabbed()
    {
        return isGrabbed;
    }


    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
    }
    public void SetStunned(bool stunned)
    {
        isStunned = stunned;
    }

    public void SetCanMove(bool canMove)
    {
        isCanMove = canMove;
    }

    public void SetGrabbed(bool grabbed)
    {
        isGrabbed = grabbed;
    }
}
