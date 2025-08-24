using System.Collections;
using UnityEngine;

public class IGroundedObstacle : MonoBehaviour
{
    CubesMarching myChunk;
    [SerializeField] Transform terrainCheckStartPosition;
    [SerializeField] LayerMask terrainLayerMask;
    [SerializeField] LayerMask grabbedGroundLayerMask;
    [SerializeField] float rayGroundedCheckLength;
    [SerializeField] float raySuffocateCheckLength;
    [SerializeField] float grabOffsetHeight;

    internal bool isOnGrabbedTerrain = false;
    internal virtual void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(terrainCheckStartPosition.position, Vector3.down, out hit, rayGroundedCheckLength, terrainLayerMask))
        {
            Debug.Log("Connected To Terrain!");
            myChunk = hit.transform.gameObject.GetComponent<CubesMarching>();

            Vector3 normal = hit.normal;
            Quaternion rotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
            transform.rotation = rotation;

            if (myChunk != null)
            {
                myChunk.RegisterMeshDeformListener(OnTerrainDeform);
            }
        }
    }
    public virtual void SetNewTerrain(ObjectActionHandler objectActionHandler)
    {
        RaycastHit hit;
        isOnGrabbedTerrain = true;
        if (Physics.Raycast(terrainCheckStartPosition.position + Vector3.up * 2, Vector3.down, out hit, rayGroundedCheckLength * 2, grabbedGroundLayerMask))
        {
            Vector3 normal = hit.normal;
            Quaternion rotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
            transform.rotation = rotation;

            transform.position = hit.point + hit.normal * grabOffsetHeight;
            Debug.Log("Grounded on " + hit.transform.gameObject.name);
        }
    }

    private void OnTerrainDeform()
    {
        if (isOnGrabbedTerrain)
        {
            return;
        }
        if(terrainCheckStartPosition == null)
        {
            return;
        }
        if (!Physics.Raycast(terrainCheckStartPosition.position, Vector3.down, rayGroundedCheckLength, terrainLayerMask) || Physics.Raycast(terrainCheckStartPosition.position, Vector3.up, raySuffocateCheckLength, terrainLayerMask))
        {
            Debug.Log("no longer grounded");
            OnGroundInvalidate();
        }
    }

    internal virtual void OnGroundInvalidate()
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if(myChunk != null)
        {
            myChunk.UnregisterMeshDeformListener(OnTerrainDeform);
        }
    }

    public virtual void OnGrabGround()
    {
        if(myChunk != null)
        {
            myChunk.UnregisterMeshDeformListener(OnTerrainDeform);
        }
    }
}
