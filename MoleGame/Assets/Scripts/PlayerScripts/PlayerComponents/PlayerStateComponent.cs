using DG.Tweening;
using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerStateComponent : IPlayerComponent
{
    [SerializeField] private bool isStunned;
    [SerializeField] private bool isCanMove;
    [SerializeField] private bool isAlwaysGrounded;
    [SerializeField] private bool isGrabbed;
    [SerializeField] private bool isDead;


    [field: SerializeField] public float ragdollWeightChange { get; private set; }

    private bool isRagdoll;

    private int ragdollRequestCounter = 0;
    private PlayerGroundedComponent groundedComponent;
    internal PointerScript pointer;

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
        pointer.HideCrown();
        base.Start();
    }
    internal override void ResetStatsOnRespawn()
    {
        
        transform.DOKill();

        DisableRagdoll();

        transform.localScale = transformBaseScale;
        transform.localRotation = transformBaseRotation;
        isAlwaysGrounded = false;
        isDead = false;
        GetComponent<Rigidbody>().mass = rbBaseMass;
        GetComponent<Rigidbody>().freezeRotation = true;
        ragdollRequestCounter = 0;
    }

    public void EnableRagdoll()
    {
        if(isRagdoll) 
            return;

        if (ragdollRequestCounter < 0)
        {
            ragdollRequestCounter = 0;
        }

        ragdollRequestCounter++;

        if (ragdollRequestCounter >= 0)
        {
            GetComponent<Rigidbody>().freezeRotation = false;
            GetComponent<Rigidbody>().mass *= ragdollWeightChange;

            isRagdoll = true;
        }

        Debug.Log("Ragdoll Rqs: " + ragdollRequestCounter);
    }
    public void DisableRagdoll()
    {
        ragdollRequestCounter--;

        if(!isRagdoll)
            return;

        if(ragdollRequestCounter < 0)
        {
            ragdollRequestCounter = 0;
        }
        if(ragdollRequestCounter <= 0)
        {
            GetComponent<Rigidbody>().freezeRotation = true;

            var lookDir = GetComponent<Rigidbody>().linearVelocity;
            lookDir.y = 0;

            Quaternion lookQuart;
            if (lookDir != Vector3.zero)
            {
                lookQuart = Quaternion.LookRotation(lookDir);
            }
            else
            {
                lookQuart = Quaternion.identity;
            }

            GetComponent<Rigidbody>().MoveRotation(lookQuart);
            GetComponent<Rigidbody>().mass /= ragdollWeightChange;

            isRagdoll = false;
        }
        Debug.Log("Ragdoll Rqs: " + ragdollRequestCounter);

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

    public bool isRagdolled()
    {
        return isRagdoll;
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
