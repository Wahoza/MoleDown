using UnityEngine;

public class DeformTerrainOnActionComponent : IActionComponent
{
    [SerializeField] public float range;
    [SerializeField] private float intensity;
    [SerializeField] bool collisionWithKinematicRB;
    public override void DoAction()
    {
        DoDeform();
    }
    void DoDeform()
    {
        if (collisionWithKinematicRB)
        {
            LevelMarchingPlatform.Instance.DeformSphereAt(transform.position, range, intensity);
        }
        else if (GetComponent<Rigidbody>() is var rb && rb != null && !rb.isKinematic)
        {
            LevelMarchingPlatform.Instance.DeformSphereAt(transform.position, Mathf.CeilToInt(range), intensity);
        }
    }
}
