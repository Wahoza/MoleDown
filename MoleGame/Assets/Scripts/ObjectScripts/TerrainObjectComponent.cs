using DG.Tweening;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class TerrainObjectComponent : MonoBehaviour
{
    [SerializeField] float cubeToMassConversion;
    [SerializeField] float baseWeight;
    [SerializeField] float extraEffectRange;

    [Header("OnTerrainGrab")]
    [SerializeField] float terrainGrabRadiusModifier;
    [SerializeField] float moveHoldTime;
    [SerializeField] Vector3 extraGrabOffset;
    [SerializeField] LayerMask grabbableModLayer;

    private float maxDist = 100;
    private float grabEffectRadius;
    private Vector3 grabEffectCenter;
    public void Initialize()
    {
        GetComponent<ObjectActionHandler>()?.RegisterGrabAction(OnGrab);
    }
    public void SetRigidbodyMassByCubeCount(int confirmedCubes)
    {
        Debug.Log(confirmedCubes);
        GetComponent<Rigidbody>().mass = baseWeight + (float)confirmedCubes * cubeToMassConversion;
    }

    public void SetRadius(float radius)
    {

        grabEffectRadius = radius + terrainGrabRadiusModifier;
        grabEffectCenter = new Vector3(0, radius, 0);

        var newRadius = radius + extraEffectRange;

        if(GetComponent<DeformTerrainOnActionComponent>() is var dftoc && dftoc != null)
        {
            dftoc.range = newRadius;
        }

        if (GetComponent<KnockbackOnActionComponent>() is var kboc && kboc != null)
        {
            kboc.range = newRadius;
        }
    }

    private void OnGrab(bool grabbed)
    {
        if(!grabbed) return;
        Debug.Log("Grabbed");

        Transform pocketTarget = GetComponent<ObjectGrabbableComponent>().pocketTransform;


        var hits = Physics.SphereCastAll(grabEffectCenter + transform.position, grabEffectRadius, transform.forward, maxDist, grabbableModLayer);

        foreach (var hit in hits)
        {
            Debug.Log(hit.transform.gameObject.layer);

            if(hit.transform == pocketTarget.parent.parent)
                continue;

            Vector3 offset = hit.transform.position - transform.position;
            hit.transform.DOMove(pocketTarget.position + offset + extraGrabOffset, moveHoldTime).SetEase(Ease.InQuad);
        }
    }

    private void OnDestroy()
    {
        GetComponent<ObjectActionHandler>()?.UnregisterGrabAction(OnGrab);
    }
}
