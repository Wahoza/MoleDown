using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalPlayerPositionComponent : MonoBehaviour
{
    private RaycastHit hit;
    [SerializeField] Vector3 DecalOffset;
    [SerializeField] Vector3 DecalDrawOffset;
    [SerializeField] float maxDownDist;
    private Vector3 searchPosition;

    void FixedUpdate()
    {
        searchPosition = transform.parent.position + DecalOffset;

        if (Physics.Raycast(searchPosition, Vector3.down, out hit, maxDownDist))
        {
            transform.position = hit.point + DecalDrawOffset;
        }
    }
}
