using DG.Tweening;
using System;
using UnityEngine;

public class PlayerStateComponent : IPlayerComponent
{
    [SerializeField] private bool isStunned;
    [SerializeField] private bool isCanMove;
    [SerializeField] private bool isAlwaysGrounded;
    [SerializeField] private bool isGrabbed;
    [SerializeField] private bool isDead;

    private PlayerGroundedComponent groundedComponent;
    private PointerScript pointer;

    private Vector3 transformBaseScale;
    private float rbBaseMass;
    private Quaternion transformBaseRotation;
    internal override void Start()
    {

        transformBaseScale = transform.localScale;
        rbBaseMass = GetComponent<Rigidbody>().mass;
        transformBaseRotation = transform.localRotation;
        isDead = false;
        pointer.ShowAlive();
        base.Start();
    }
    internal override void ResetStatsOnRespawn()
    {
        transform.DOKill();
        transform.localScale = transformBaseScale;
        transform.localRotation = transformBaseRotation;
        isAlwaysGrounded = false;
        isDead = false;
        GetComponent<Rigidbody>().mass = rbBaseMass;
    }
    public void SetPlayerPointer(PointerScript newPointer)
    {
        pointer = newPointer;
    }
    public void JoinComponent(MonoBehaviour component)
    {
        if (component is PlayerGroundedComponent pgc)
        {
            groundedComponent = pgc;
        }
    }
    public bool IsGrounded()
    {
        if(groundedComponent != null)
        {
            return groundedComponent.GetGrounded() || isAlwaysGrounded;
        }
        return true;
    }

    public bool CanMove()
    {
        return isCanMove;
    }
    public bool IsDead()
    {
        return isDead;
    }
    public bool IsStunned()
    {
        return isStunned;
    }
    public bool IsGrabbed()
    {
        return isGrabbed;
    }

    public void SetAlwaysGrounded(bool alwaysGrounded)
    {
        isAlwaysGrounded = alwaysGrounded;

    }

    public void SetStunned(bool stunned)
    {
        isStunned = stunned;
    }

    public void SetIsDead(bool isDead)
    {
        if (isDead)
        {
            pointer.ShowDead();
        }
        else
        {
            pointer.ShowAlive();
        }
        this.isDead = isDead;
    }
    public void SetCanMove(bool canMove)
    {
        isCanMove = canMove;
    }

    public void SetGrabbed(bool grabbed)
    {
        isGrabbed = grabbed;
    }

    internal override void OnDestroy()
    {
        if(pointer != null)
        {
            Destroy(pointer.gameObject);
        }
        base.OnDestroy();
    }
}
