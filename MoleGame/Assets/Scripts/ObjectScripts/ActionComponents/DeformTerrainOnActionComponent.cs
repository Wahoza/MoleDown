using UnityEngine;

public class DeformTerrainOnActionComponent : IActionComponent
{
    [SerializeField] public float range;
    [SerializeField] private float intensity;
    [SerializeField] bool collisionWithKinematicRB;
    [SerializeField] LayerMask terrainLayer;
    public override void DoAction()
    {
        DoDeform();
    }
    void DoDeform()
    {
        if (collisionWithKinematicRB)
        {
            DeformTerrain();
        }
        else if (GetComponent<Rigidbody>() is var rb && rb != null && !rb.isKinematic)
        {
            DeformTerrain();
        }
    }

    void DeformTerrain()
    {
        if(Physics.OverlapSphere(transform.position, range, terrainLayer).Length > 0)
        {
            LevelMarchingPlatform.Instance.DeformSphereAt(transform.position, Mathf.CeilToInt(range), intensity);
        }

    }
}
