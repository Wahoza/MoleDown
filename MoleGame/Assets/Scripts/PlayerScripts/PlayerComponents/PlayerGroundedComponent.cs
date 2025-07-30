using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlayerStateComponent), typeof(PlayerActionHandler))]
public class PlayerGroundedComponent : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] BoxCollider FeetBox;
    [SerializeField] int alreadyExistingCols; // note this is for collisions with our character
    [SerializeField] LayerMask canStandLayers;

    [Header("Function Related")]
    [SerializeField] float landBufferTime;

    private bool isGrounded = true;
    private bool lastIsGrounded = true;

    private float lastWasGroundedTime;
    private void Start()
    {
        GetComponent<PlayerStateComponent>().JoinComponent(this);
    }
    public bool GetGrounded()
    {
        return isGrounded;
    }
    private void FixedUpdate()
    {
        if(FeetBox == null) return;

        isGrounded = false;
        var colliders = Physics.OverlapBox(FeetBox.transform.position, FeetBox.size / 2, transform.rotation, canStandLayers);

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
                Debug.Log("Landing!");
                GetComponent<PlayerActionHandler>().OnLand(Time.time - lastWasGroundedTime);
            }
        }

        lastIsGrounded = isGrounded;
    }
    public void Respawn()
    {
        isGrounded = true;
    }
}
