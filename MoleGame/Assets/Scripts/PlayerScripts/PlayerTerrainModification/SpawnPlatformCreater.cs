using System.Collections;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class SpawnPlatformCreater : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;

        SpawnPlatform();
    }
    void SpawnPlatform()
    {
        var bounds = GetComponent<BoxCollider>().bounds;
        bounds.size = bounds.size * 0.3f;
        LevelMarchingPlatform.instance.DeformCubeAt(bounds, intensity: 4);

        gameObject.SetActive(false);
    }
}
