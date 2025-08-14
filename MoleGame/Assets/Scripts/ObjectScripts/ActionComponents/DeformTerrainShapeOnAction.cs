using UnityEngine;

public class DeformTerrainShapeOnAction : IActionComponent
{
    [SerializeField] GameObject collisionShapeHolder;
    [SerializeField] float intensity;
    [SerializeField] DeformationType type;
    enum DeformationType
    {
        Sphere,
        Cylinder,
        Cube
    }
    public override void DoAction()
    {
        switch (type)
        {
            case DeformationType.Sphere:
                if (collisionShapeHolder.GetComponent<SphereCollider>() is var sc && sc != null)
                {
                    LevelMarchingPlatform.Instance.DeformSphereAt(sc.center, sc.radius, intensity);
                }
                else
                {
                    Debug.LogWarning("Sphere deformations must have sphere collider component in collisionShapeHolder");
                }
                return;
            case DeformationType.Cylinder:
                if (collisionShapeHolder.GetComponent<CapsuleCollider>() is var cc && cc != null)
                {
                    var radius = cc.radius;
                    var height = cc.height;
                    LevelMarchingPlatform.Instance.DeformCylinderAt(cc.bounds.center, radius, height, intensity);
                }
                else
                {
                    Debug.LogWarning("Cylinder deformations must have capsule component in collisionShapeHolder");
                }
                return;
            case DeformationType.Cube:
                if (collisionShapeHolder.GetComponent<BoxCollider>() is var bc && bc != null)
                {
                    var bounds = bc.bounds;
                    bounds.size *= 0.3f;
                    LevelMarchingPlatform.Instance.DeformCubeAt(bounds, intensity);
                }
                else
                {
                    Debug.LogWarning("Cube deformations must have boxcollider component in collisionShapeHolder");
                }
                return;

        }
        

        
    }
}
