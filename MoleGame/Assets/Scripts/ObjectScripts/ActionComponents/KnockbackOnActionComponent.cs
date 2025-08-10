using UnityEngine;

public class KnockbackOnActionComponent : IActionComponent
{
    [SerializeField] public float range;
    [SerializeField] float knockBackForce;
    [SerializeField] bool collisionWithKinematicRB;
    [SerializeField] LayerMask knockBackLayerMask;
    //private void OnCollisionEnter(Collision collision)
    //{
    //    DoCollision(collision.contacts[0].point);
    //}    
    //private void OnCollisionStay(Collision collision)
    //{
    //    DoCollision(collision.contacts[0].point);
    //}
    public override void DoAction()
    {
        DoCollision(transform.position);
    }
    void DoCollision(Vector3 collisionPoint)
    {
        if (collisionWithKinematicRB)
        {
            var hits = Physics.SphereCastAll(collisionPoint, range, transform.forward, knockBackLayerMask);

            foreach (var hit in hits)
            {
                if (hit.transform == transform)
                    continue;

                if (hit.transform.GetComponent<Rigidbody>() is var rb && rb != null) 
                {
                    var force = -(hit.point - collisionPoint).normalized * knockBackForce;
                    force.y = Mathf.Abs(force.y);

                    rb.AddForce(force);
                }
            }
        }
        else if (GetComponent<Rigidbody>() is var rb && rb != null && !rb.isKinematic)
        {
            var hits = Physics.SphereCastAll(collisionPoint, range, transform.forward, knockBackLayerMask);


            foreach (var hit in hits)
            {
                if (hit.transform == transform)
                    continue;

                if (hit.transform.GetComponent<Rigidbody>() is var hitrb && hitrb != null)
                {
                    var force = -(hit.point - collisionPoint).normalized * knockBackForce;
                    force.y = Mathf.Abs(force.y);

                    hitrb.AddForce(force);
                }
            }
        }
    }
}
