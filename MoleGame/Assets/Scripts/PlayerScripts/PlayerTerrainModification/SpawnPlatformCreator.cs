using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnPlatformCreator : MonoBehaviour
{
    [SerializeField] BoxCollider box;
    private bool spawnedPlatform;
    public async Task SpawnPlatformAsync(bool updateMeshes)
    {
        box.gameObject.SetActive(true);

        Debug.Log("SpawningPlatform");
        var bounds = box.bounds;
        bounds.size = bounds.size * 0.4f;

        await LevelMarchingPlatform.Instance.DeformCubeAt(bounds, intensity: 4);

        if (updateMeshes)
        {
            LevelMarchingPlatform.Instance.UpdateMeshes();
        }


        spawnedPlatform = true;

        box.gameObject.SetActive(false);
    }

    public async void SpawnPlatform(bool updateMeshes)
    {
        await SpawnPlatformAsync(updateMeshes);
    }

    public IEnumerator SpawnPlatformWithWait()
    {
        spawnedPlatform = false;
        SpawnPlatformAsync(false);

        while (!spawnedPlatform)
        {
            yield return null;
        }
    }
}
