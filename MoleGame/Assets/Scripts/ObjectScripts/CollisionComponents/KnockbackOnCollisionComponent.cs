using UnityEngine;

public class KnockbackOnCollisionComponent : MonoBehaviour
{
    [SerializeField] public float range;
    [SerializeField] float knockBackForce;
    [SerializeField] bool collisionWithKinematicRB;
    [SerializeField] LayerMask knockBackLayerMask;
    private void OnCollisionEnter(Collision collision)
    {
        DoCollision(collision.contacts[0].point);
    }

    void DoCollision(Vector3 collisionPoint)
    {
        if (collisionWithKinematicRB)
        {
            var hits = Physics.SphereCastAll(collisionPoint, range, transform.forward, knockBackLayerMask);

            foreach (var hit in hits)
            {
                if (hit.transform.GetComponent<Rigidbody>() is var rb && rb != null) 
                { 
                    rb.AddForce((hit.point - collisionPoint).normalized * knockBackForce);
                }
            }
        }
        else if (GetComponent<Rigidbody>() is var rb && rb != null && !rb.isKinematic)
        {
            var hits = Physics.SphereCastAll(collisionPoint, range, transform.forward, knockBackLayerMask);

            foreach (var hit in hits)
            {
                if (hit.transform.GetComponent<Rigidbody>() is var hitrb && hitrb != null)
                {
                    hitrb.AddForce((hit.point - collisionPoint).normalized * knockBackForce);
                }
            }
        }
    }
}
