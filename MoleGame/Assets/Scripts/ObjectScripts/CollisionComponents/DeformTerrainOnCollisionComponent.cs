using UnityEngine;

public class DeformTerrainOnCollisionComponent : MonoBehaviour
{
    [SerializeField] public float range;
    [SerializeField] private float intensity;
    [SerializeField] bool collisionWithKinematicRB;
    private void OnCollisionEnter(Collision collision)
    {
        DoCollision();
    }

    void DoCollision()
    {
        if (collisionWithKinematicRB)
        {
            LevelMarchingPlatform.instance.DeformSphereAt(transform.position, range, intensity);
        }
        else if (GetComponent<Rigidbody>() is var rb && rb != null && !rb.isKinematic)
        {
            LevelMarchingPlatform.instance.DeformSphereAt(transform.position, Mathf.CeilToInt(range), intensity);
        }
    }
}
