using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObstacle : IGroundedObstacle
{
    [Header("Tree Setup")]
    [SerializeField] Rigidbody treerb;
    [SerializeField] GameObject groundedTreeModel;
    [SerializeField] GameObject uprootedTreeModel;
    [SerializeField] float lerpToZeroStrength = 0.6f;

    [Header("Deformation")]
    [SerializeField] float deformationTimeStep = 0.3f;

    [SerializeField] private  List<DeformTerrainShapeOnAction> deformationComponents;
    [SerializeField] private int maxDeformsPerUpdate = 2;

    private Coroutine deformationRoutine;

    internal override void Start()
    {
        base.Start();

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, lerpToZeroStrength);

        GetComponent<ObjectActionHandler>().RegisterGrabAction(OnTreeGrabbed);
    }

    void OnTreeGrabbed(bool b)
    {
        SwitchToFallenTree();
    }

    public void SwitchToFallenTree()
    {
        groundedTreeModel.gameObject.SetActive(false);
        uprootedTreeModel.gameObject.SetActive(true);


        if (deformationRoutine == null) { 
            deformationRoutine = StartCoroutine(OnTreeFallenRoutine());
        }
        treerb.isKinematic = false;
    }
    internal override void OnGroundInvalidate()
    {
        SwitchToFallenTree();
    }

    private IEnumerator OnTreeFallenRoutine()
    {
        float elapsed = 0;

        while (true)
        {
            yield return null;

            if (elapsed > deformationTimeStep)
            {
                elapsed = 0;

                for (int i = 0; i < deformationComponents.Count; i++)
                {
                    deformationComponents[i].DoAction();

                    if(maxDeformsPerUpdate > 0)
                    {
                        if(i % maxDeformsPerUpdate == 0)
                        {
                            yield return new WaitForFixedUpdate();
                        }
                    }
                }
            }

            elapsed += Time.deltaTime;
        }
    }
}
