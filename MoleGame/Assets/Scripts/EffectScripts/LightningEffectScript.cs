using System.Collections.Generic;
using System.Collections;
using System.Linq;

using UnityEngine;

public class LightningEffectScript : MonoBehaviour
{
    [Header("LightningSetup")]
    [SerializeField] List<Transform> lightningLights;
    [SerializeField] float minTimeBetweenLight;
    [SerializeField] float maxTimeBetweenLight;
    [SerializeField] int minLightUseCount;
    [SerializeField] int maxLightUseCount;

    [Header("LighningSpawning")]
    [SerializeField] float minTimeBetweenLightning;
    [SerializeField] float maxTimeBetweenLightning;
    
    private IEnumerator LightningUpdate()
    {
        while (true)
        {
            float waitTime = Random.Range(minTimeBetweenLightning, maxTimeBetweenLightning);

            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(DoLightning());
        }
    }

    private IEnumerator DoLightning()
    {
        int lightningCount = Random.Range(minLightUseCount, maxLightUseCount);

        var lightningsThisLightning = lightningLights.OrderBy(x => Random.value)
            .Take(lightningCount)
            .ToList();

        foreach (var light in lightningsThisLightning)
        {
            float waitTime = Random.Range(minTimeBetweenLight, maxTimeBetweenLight);
            light.gameObject.SetActive(true);

            yield return new WaitForSeconds(waitTime);

            light.gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        StartCoroutine(LightningUpdate());
    }
    private void OnDisable()
    {
        StopAllCoroutines();

        foreach (var light in lightningLights)
        {
            gameObject.SetActive(false);
        }
    }
}
