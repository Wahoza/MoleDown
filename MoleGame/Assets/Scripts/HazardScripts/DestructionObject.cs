using UnityEngine;

[CreateAssetMenu(fileName = "DestructionObject", menuName = "Scriptable Objects/DestructionObject")]
public class DestructionObject : ScriptableObject
{
    enum Type
    {
        Sphere,
        Cylinder,
        Cube
    }
    [SerializeField] float radius;
    [SerializeField] float width;
    [SerializeField] float height;
    [SerializeField] float intensity;
    [SerializeField] Type destructionType;

    public void Deform(MarchingWorld target, Vector3 worldPos)
    {
        switch (destructionType)
        {
            case Type.Sphere:
                target.DeformSphereAt(worldPos, radius, intensity);
                return;
            case Type.Cylinder:
                target.DeformCylinderAt(worldPos, width, height, intensity);
                return;
            case Type.Cube:
                target.DeformCubeAt(new Bounds(worldPos, new Vector3(width, height, width)), intensity);

                return;
        }
    }
}
