using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class TerrainObjectComponent : MonoBehaviour
{
    [SerializeField] float cubeToMassConversion;
    [SerializeField] float baseWeight;
    [SerializeField] float extraEffectRange;
    public void SetRigidbodyMassByCubeCount(int confirmedCubes)
    {
        Debug.Log(confirmedCubes);
        GetComponent<Rigidbody>().mass = baseWeight + (float)confirmedCubes * cubeToMassConversion;
    }

    public void SetRadius(float radius)
    {
        Debug.Log(radius);
        var newRadius = radius + extraEffectRange;

        if(GetComponent<DeformTerrainOnCollisionComponent>() is var dftoc && dftoc != null)
        {
            dftoc.range = newRadius;
        }

        if (GetComponent<KnockbackOnCollisionComponent>() is var kboc && kboc != null)
        {
            kboc.range = newRadius;
        }
    }
}
