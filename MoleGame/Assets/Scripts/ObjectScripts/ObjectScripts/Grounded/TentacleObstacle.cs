using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
public class TentacleObstacle : IGroundedObstacle
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Idle Perlin Setup")]
    [SerializeField] Transform firstRBHolder;
    [SerializeField] float maxVelocity;
    [SerializeField] float perlinEffectVelocity;
    [SerializeField] float perlinScrollSpeed;
    [SerializeField] float maxNoiseOffset;
    [SerializeField] float jointOffset;
    [SerializeField] float jointUpVelocity;

    private List<Coroutine> idleTentaclesPhysics = new();
    private Vector2 xnoiseOffset;
    private Vector2 znoiseOffset;
    
    internal override void Start()
    {
        base.Start();

        var rbs = firstRBHolder.GetComponentsInChildren<Rigidbody>();

        if (rbs.Count() <= 0)
        {
            return;
        }
        
        xnoiseOffset = Random.value * maxNoiseOffset * Vector2.one;
        znoiseOffset = Random.value * maxNoiseOffset * Vector2.one;

        for (int i = 0; i < rbs.Length; i++)
        {
            idleTentaclesPhysics.Add(StartCoroutine(IdleTentaclePhysicRoutine(rbs[i], i)));
        }
    }
    private IEnumerator IdleTentaclePhysicRoutine(Rigidbody rb, float index)
    {
        float elapsedTime = 0;
        while (true)
        {
            float xCurrentNoise = Mathf.PerlinNoise(xnoiseOffset.x, xnoiseOffset.y + elapsedTime * perlinScrollSpeed + index * jointOffset);
            xCurrentNoise -= 0.5f;
            float zCurrentNoise = Mathf.PerlinNoise(znoiseOffset.x, znoiseOffset.y + elapsedTime * perlinScrollSpeed + index * jointOffset);
            zCurrentNoise -= 0.5f;

            rb.AddForce(new Vector3 ( xCurrentNoise * perlinEffectVelocity, jointUpVelocity, zCurrentNoise * perlinEffectVelocity ) * Time.deltaTime, ForceMode.Force);

            if (rb.linearVelocity.sqrMagnitude > Mathf.Pow(maxVelocity, 2))
            {
                var newVelocity = rb.linearVelocity.normalized * maxVelocity;
                newVelocity.y = jointOffset;
                rb.linearVelocity = newVelocity;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
