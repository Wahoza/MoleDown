using UnityEngine;

public class DeformTerrainShapeOnAction : IActionComponent
{
    [SerializeField] GameObject collisionShapeHolder;
    [SerializeField] float intensity;
    [SerializeField] DeformationType type;
    [SerializeField] bool doEffect = true;
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
                    LevelMarchingPlatform.Instance.DeformSphereAt(sc.bounds.center, sc.radius, intensity, doEffect);
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
                    LevelMarchingPlatform.Instance.DeformCylinderAt(cc.bounds.center, radius, height, intensity, doEffect);
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
                    LevelMarchingPlatform.Instance.DeformCubeAt(bounds, intensity, doEffect);
                }
                else
                {
                    Debug.LogWarning("Cube deformations must have boxcollider component in collisionShapeHolder");
                }
                return;

        }
        

        
    }
}
