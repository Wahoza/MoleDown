using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloaterScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody rb;
    public float depthBefSub;
    public float displacementAmt;
    public int floaters;

    public float waterDrag;
    public float waterAngularDrag;
    public float floatieness;

    public LayerMask waterLayer;
    Collider[] colliders;

    void FixedUpdate()
    {

        colliders = Physics.OverlapSphere(transform.position, 0.1f, waterLayer, QueryTriggerInteraction.Collide);

        foreach(Collider collider in colliders)
        {
            rb.AddForceAtPosition(Physics.gravity / floaters, transform.position, ForceMode.Acceleration);


            float projectedPosY = collider.bounds.max.y;

            if (transform.position.y < projectedPosY)
            {
                float displacementMulti = Mathf.Clamp01((projectedPosY - transform.position.y) / depthBefSub) * displacementAmt;

                rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMulti, 0f), transform.position, ForceMode.Acceleration);
                rb.AddForce(displacementMulti * -rb.linearVelocity * waterDrag * Time.fixedDeltaTime * floatieness, ForceMode.VelocityChange);
                rb.AddTorque(displacementMulti * -rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }

    }

}
