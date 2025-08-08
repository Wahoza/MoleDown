using System.Collections.Generic;
using UnityEngine;

public class FloaterHolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] List<FloaterScript> Floaters;
    [SerializeField] LayerMask waterLayer;
    [SerializeField] public float depthBefSub;
    [SerializeField] public float displacementAmt;
    [SerializeField] public int floaters;

    [SerializeField] public float waterDrag;
    [SerializeField] public float waterAngularDrag;
    [SerializeField] public float floatieness;

    void Start()
    {
        Rigidbody rb = transform.parent.GetComponent<Rigidbody>();

        foreach (FloaterScript floater in Floaters)
        {
            floater.waterLayer = waterLayer;
            floater.rb = rb;
            floater.depthBefSub = depthBefSub;
            floater.displacementAmt = displacementAmt;
            floater.floaters = floaters;
            floater.waterDrag = waterDrag;
            floater.waterAngularDrag = waterAngularDrag;
            floater.floatieness = floatieness;
        }

        transform.parent.GetComponent<PlayerActionHandler>().RegisterDeathAction(EnableFloating);
        transform.parent.GetComponent<PlayerActionHandler>().RegisterSpawnAction(DisableFloaters);

        DisableFloaters();
    }
    
    void EnableFloating()
    {
        foreach(FloaterScript floater in Floaters)
        {
            floater.gameObject.SetActive(true);
            floater.enabled = true;
        }
    }

    void DisableFloaters()
    {
        foreach (FloaterScript floater in Floaters)
        {
            floater.gameObject.SetActive(false);
            floater.enabled = false;
        }
    }

    private void OnDestroy()
    {
        transform.parent.GetComponent<PlayerActionHandler>().UnregisterDeathAction(EnableFloating);
        transform.parent.GetComponent<PlayerActionHandler>().UnregisterSpawnAction(DisableFloaters);
    }
}
