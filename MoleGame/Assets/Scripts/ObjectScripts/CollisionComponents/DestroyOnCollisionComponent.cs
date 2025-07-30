using System.Collections;
using UnityEngine;

public class DestroyOnCollisionComponent : MonoBehaviour
{
    [SerializeField] float timeUntilDestruction;
    [SerializeField] bool collisionWithKinematicRB;

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionWithKinematicRB)
        {
            StartCoroutine(DestroyAfterTime());
        }
        else if (GetComponent<Rigidbody>() is var rb && rb != null && !rb.isKinematic)
        {
            StartCoroutine(DestroyAfterTime());
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(timeUntilDestruction);

        Destroy(gameObject);
    }
}
