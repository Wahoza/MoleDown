using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class SpawnPlatformCreator : MonoBehaviour
{
    public async Task SpawnPlatformAsync(bool updateMeshes)
    {
        Debug.Log("SpawningPlatform");
        var bounds = GetComponent<BoxCollider>().bounds;
        bounds.size = bounds.size * 0.4f;

        await LevelMarchingPlatform.Instance.DeformCubeAt(bounds, intensity: 4);

        if (updateMeshes)
        {
            LevelMarchingPlatform.Instance.UpdateMeshes();
        }
    }

    public async void SpawnPlatform(bool updateMeshes)
    {
        await SpawnPlatformAsync(updateMeshes);
    }
}
