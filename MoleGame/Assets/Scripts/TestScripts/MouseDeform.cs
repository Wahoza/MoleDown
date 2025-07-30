using System.Collections;
using UnityEngine;

public class MouseDeform : MonoBehaviour
{
    enum DeformType
    {
        Sphere,
        Cylinder,
        Cube
    }
    [SerializeField] private float deformRadius = 2f;
    [SerializeField] private float deformHeight = 10f;
    [SerializeField] private float deformStrength = 0.4f;
    [SerializeField] private LayerMask chunkLayerMask;
    [SerializeField] private DeformType deform;


    [SerializeField] 
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, chunkLayerMask))
            {
                var parent = hit.collider.transform.parent;
                if (parent != null)
                {
                    var platform = parent.GetComponent<LevelMarchingPlatform>();
                    if(platform != null)
                    {
                        switch (deform)
                        {
                            case DeformType.Sphere:
                                platform.DeformSphereAt(hit.point, deformRadius, deformStrength);

                                return;
                            case DeformType.Cylinder:
                                platform.DeformCylinderAt(hit.point, deformRadius, deformHeight, deformStrength);

                                return;
                            case DeformType.Cube:
                                platform.DeformCubeAt(new Bounds(hit.point, new Vector3(deformRadius, deformHeight, deformRadius)), -deformStrength);

                                return;
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) // Left-click
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, chunkLayerMask))
            {
                var parent = hit.collider.transform.parent;
                if (parent != null)
                {
                    var platform = parent.GetComponent<LevelMarchingPlatform>();
                    if (platform != null)
                    {
                        switch (deform)
                        {
                            case DeformType.Sphere:
                                platform.DeformSphereAt(hit.point, deformRadius, -deformStrength);

                                return;
                            case DeformType.Cylinder:
                                platform.DeformCylinderAt(hit.point, deformRadius, deformHeight, -deformStrength);

                                return;
                            case DeformType.Cube:
                                platform.DeformCubeAt(new Bounds(hit.point, new Vector3(deformRadius, deformHeight, deformRadius)), -deformStrength);

                                return;
                        }
                    }
                }
            }
        }
    }
}