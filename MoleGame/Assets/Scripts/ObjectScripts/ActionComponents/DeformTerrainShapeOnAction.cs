using UnityEngine;

public class DeformTerrainShapeOnAction : IActionComponent
{
    [SerializeField] GameObject collisionShapeHolder;
    [SerializeField] float intensity;

    public override void DoAction()
    {
        if(collisionShapeHolder.GetComponent<BoxCollider>() is var bc && bc != null)
        {
            var bounds = bc.bounds;
            bounds.size *= 0.3f;
            LevelMarchingPlatform.Instance.DeformCubeAt(bounds, intensity);
        }

        if(collisionShapeHolder.GetComponent<SphereCollider>() is var sc && sc != null)
        {
            LevelMarchingPlatform.Instance.DeformSphereAt(sc.center, sc.radius, intensity);
        }
    }
}
