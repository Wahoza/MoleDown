using System.Collections;
using UnityEngine;

public class PlayerMaximumVelocityComponent : IPlayerComponent
{
    [SerializeField] float maxVelocity;
    [SerializeField] float velocityIncreasePastMax;
    [SerializeField] float baseVelocityDecrease;

    private Coroutine limitRoutine;
    private Rigidbody body;
    internal override void Start()
    {
        
        base.Start();
        if (body == null)
        {
            body = GetComponent<Rigidbody>();
        }
        if (limitRoutine == null)
        {
            limitRoutine = StartCoroutine(VelocityLimiterRoutine());
        }
        
    }

    internal override void ResetStatsOnRespawn()
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody>();
        }

        if (limitRoutine == null)
        {
            limitRoutine = StartCoroutine(VelocityLimiterRoutine());
        }
        
        base.ResetStatsOnRespawn();
    }

    private IEnumerator VelocityLimiterRoutine()
    {
        while (true)
        {
            if (body != null && !body.isKinematic)
            {
                if(body.linearVelocity.sqrMagnitude > maxVelocity * maxVelocity)
                {
                    Vector3 normalizedVel = body.linearVelocity.normalized;
                    float length = body.linearVelocity.magnitude;
                    
                    length = length - ((velocityIncreasePastMax * (length - maxVelocity) - baseVelocityDecrease) * Time.deltaTime);

                    Vector3 resultVel = normalizedVel * length;

                    body.linearVelocity = resultVel;    
                }
            }
            yield return null;
        }
    }
}
