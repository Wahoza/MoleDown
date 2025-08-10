using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Collections;
public class MeteorScript : MonoBehaviour
{
    [SerializeField] float minRandomSize;
    [SerializeField] float maxRandomSize;
    [SerializeField] AnimationCurve distributionCurve;
    [SerializeField] float destroyRadiusIncrease;
    [SerializeField] float knockBackRadiusIncrease;
    [Header ("Visuals")]
    [SerializeField] Transform modelController;
    [SerializeField] DecalProjector projector;
    [SerializeField] float projectorExpandDuration;

    [Header("Physics")]
    [SerializeField] Vector3 gravityDirection;
    [SerializeField] float gravityStrength;
    [SerializeField] Rigidbody rb;
    [SerializeField] float minY;

    private void Awake()
    {
        float radiusIncrease = minRandomSize + distributionCurve.Evaluate(Random.value) * maxRandomSize;
        transform.localScale *= radiusIncrease;

        GetComponent<DeformTerrainOnActionComponent>().range = radiusIncrease * destroyRadiusIncrease;
        GetComponent<ObjectActionHandler>().RegisterGrabAction(OnGrab);
        GetComponent<ObjectGrabbableComponent>().grabOffset = transform.localScale.y * Vector3.up;
        GetComponent<KnockbackOnActionComponent>().range = knockBackRadiusIncrease * destroyRadiusIncrease;

        var randomRotation = Random.ColorHSV();
        var adjustedRotation = new Vector3(randomRotation.r, randomRotation.b, randomRotation.g) * 360;

        modelController.eulerAngles = adjustedRotation;
        //projector.size = new Vector3(radiusIncrease, projector.size.y, radiusIncrease);

        //DOTween.To(
        //    () => projector.size.x,
        //    x => projector.size = new Vector3(x, projector.size.y, x),
        //    radiusIncrease,
        //    projectorExpandDuration
        //);

        rb.useGravity = false;

        StartCoroutine(ApplyGravity());
        StartCoroutine(DestroyWhenBelowMinY());
    }
    IEnumerator DestroyWhenBelowMinY()
    {
        while (transform.position.y > minY)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
    IEnumerator ApplyGravity()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        while (true)
        {
            rb.AddForce(gravityDirection.normalized * gravityStrength, ForceMode.Acceleration);
            yield return wait; // sync with physics step
        }
    }

    void OnGrab(bool grabbed)
    {
        rb.freezeRotation = false;
    }

    private void OnDestroy()
    {
        GetComponent<ObjectActionHandler>().UnregisterGrabAction(OnGrab);
    }
}
