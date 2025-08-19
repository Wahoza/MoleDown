using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class WormScript : MonoBehaviour
{
    [Header("Worm Body")]
    [SerializeField] AnimationCurve sizeDistribution;
    [SerializeField] float maxSize;
    [SerializeField] int minBodyLength;
    [SerializeField] int maxBodyLength;
    [SerializeField] float spawnHeight;
    [SerializeField] float bodyDistanceThreshold;
    [SerializeField] AnimationCurve bodyScaleOverSize;

    [Header("Visuals")]
    [SerializeField] GameObject BodyPrefab;
    [SerializeField] Transform head;
    [SerializeField] Transform mouth;
    [SerializeField] float mouthRotationSpeed;
    [SerializeField] float lookAheadDist;
    [SerializeField] float lookSmoothTime;

    [Header("Movement")]
    [SerializeField] AnimationCurve airAnimationCurve;
    [SerializeField] AnimationCurve groundAnimationCurve;
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;
    [SerializeField] float minDown;
    [SerializeField] float maxDown;
    [SerializeField] float moveSpeed;
    [SerializeField] float distanceAdjustment;

    [Header("MoveDuration")]
    [SerializeField] int minBounces;
    [SerializeField] int maxBounces;



    private int currentBounces;
    private int totalBounces;
    private float currentMoveHeight;

    private Vector3 currentTarget;
    private Vector3 lastTarget;

    private AnimationCurve currentAnimationCurve;
    private List<BodyPart> bodies = new();
    private List<Coroutine> bodyRoutines = new();  
    public IEnumerator Start()
    {
        float size = sizeDistribution.Evaluate(Random.value) * maxSize;
        transform.localScale *= size;

        if (GetComponent<DeformTerrainOnActionComponent>() != null) {
            GetComponent<DeformTerrainOnActionComponent>().range *= size; 
        }

        Vector3 firstTarget = ObjectManager.Instance.GetRandomValidTerrainPoint();
        if(firstTarget == -Vector3.one)
        {
            Destroy(gameObject);
            yield break;
        }

        totalBounces = Random.Range(minBounces, maxBounces);
        totalBounces += totalBounces % 2 == 1 ? 1 : 0;

        Vector3 spawnPosition = firstTarget + new Vector3(0, -firstTarget.y + spawnHeight, 0);
        transform.position = spawnPosition;
        lastTarget = spawnPosition;
        currentTarget = firstTarget;
        
        currentMoveHeight = 0;

        int bodyCount = Random.Range(minBodyLength, maxBodyLength);

        for (int i = 0; i <= bodyCount; i++)
        {
            var bodyTransform = Instantiate(BodyPrefab);

            bodyTransform.transform.localScale *= bodyScaleOverSize.Evaluate(((float)i) / ((float)bodyCount)) * size;

            bodyTransform.transform.position = spawnPosition;

            BodyPart part = new BodyPart();

            part.body = bodyTransform;
            part.animationCurves.Enqueue(groundAnimationCurve);
            part.targets.Enqueue(firstTarget);
            part.moveHeights.Enqueue(currentMoveHeight);
            part.moveSpeed = moveSpeed;
            part.lastTarget = spawnPosition;

            bodies.Add(part);
        }

        for (int i = bodyCount - 1; i >= 1; i--)
        {
            bodies[i].bodyInfront = bodies[i - 1].body.transform;
        }
        bodies[0].bodyInfront = transform;

        StartCoroutine(StartBodyRoutines());
        StartCoroutine(MouthRoutine());
        yield return UpdateRoutine();

        yield return new WaitUntil(() => bodies[bodies.Count - 1].isFinished);
        Destroy(gameObject);
    }
    private IEnumerator MouthRoutine()
    {
        while (true)
        {
            mouth.transform.Rotate(Vector3.right * Time.deltaTime * mouthRotationSpeed);
            yield return null;
        }
    }
    private IEnumerator StartBodyRoutines()
    {
        {
            float r1 = transform.localScale.x;
            float r2 = bodies[0].body.transform.localScale.x;

            float sumRadii = (r1 + r2) / 2;
            float minThreshold = sumRadii * bodyDistanceThreshold;

            yield return new WaitUntil(() =>
            {
                float distance = Vector3.Distance(transform.position, bodies[0].body.transform.position);

                return distance >= minThreshold;
            });
        }

        for (int i = 0; i < bodies.Count; i++)
        {
            bodyRoutines.Add(StartCoroutine(bodies[i].BodyUpdateRoutine()));
            bodyRoutines.Add(StartCoroutine(bodies[i].LookAtRoutine()));

            if (i + 1 < bodies.Count)
            {
                float r1 = bodies[i].body.transform.localScale.x;
                float r2 = bodies[i + 1].body.transform.localScale.x;

                float sumRadii = (r1 + r2) / 2;
                float minThreshold = sumRadii * bodyDistanceThreshold;

                yield return new WaitUntil(() =>
                {
                    float distance = Vector3.Distance(bodies[i].body.transform.position, bodies[i + 1].body.transform.position);

                    return distance >= minThreshold;
                });
            }
        }
    }

    private IEnumerator UpdateRoutine()
    {
        float elapsedTimeInPhase = 0;
        float distance = 0;
        float positionLerpValue = 1;
        float adjustedMoveSpeed = 0;

        while (true)
        {
            if(positionLerpValue >= 1)
            {
                elapsedTimeInPhase = 0;

                if ((currentBounces < totalBounces) && currentBounces != 0)
                {
                    lastTarget = currentTarget;

                    currentTarget = ObjectManager.Instance.GetRandomValidTerrainPoint();
                    currentMoveHeight = currentBounces % 2 == 0 ? Random.Range(minDown, maxDown) : Random.Range(minHeight, maxHeight);
                    
                    float adjustmentDistVal = new Vector3(currentTarget.x - lastTarget.x, 0, currentTarget.y - lastTarget.y).magnitude;
                    currentMoveHeight *= Mathf.Clamp01(adjustmentDistVal * distanceAdjustment);
                }
                else if (currentBounces >= totalBounces)
                {
                    lastTarget = currentTarget;

                    currentTarget = lastTarget + new Vector3(0, -lastTarget.y + spawnHeight, 0);

                    currentMoveHeight = 0;
                }

                distance = (currentTarget - lastTarget).magnitude;

                currentAnimationCurve = currentBounces % 2 == 0 ? groundAnimationCurve : airAnimationCurve;
                adjustedMoveSpeed = moveSpeed / (distance == 0 ? 1 : distance);

                foreach (var body in bodies)
                {
                    body.animationCurves.Enqueue(currentAnimationCurve);
                    body.moveHeights.Enqueue(currentMoveHeight);
                    body.targets.Enqueue(currentTarget);
                }

                if(currentBounces > totalBounces)
                {
                    yield break;
                }

                currentBounces++;
            }


            positionLerpValue = elapsedTimeInPhase * adjustedMoveSpeed;

            float newHeight = currentAnimationCurve.Evaluate(positionLerpValue) * currentMoveHeight;
            Vector3 evaluatedPosition = Vector3.Lerp(lastTarget, currentTarget, positionLerpValue) + Vector3.up * newHeight;
            
            transform.forward = evaluatedPosition - transform.position;
            ////if(lookAheadTimer >= lookSmoothTime)
            //{
            //    //float lookAtHeight = currentAnimationCurve.Evaluate(positionLerpValue + lookAheadDist) * currentMoveHeight;
            //    //Vector3 lookAtEvaluatedPosition = currentTarget + Vector3.up * newHeight + new Vector3(currentTarget.x - lastTarget.x, 0 , currentTarget.z - lastTarget.z);
            //    Vector3 lookAtEvaluatedPosition = transform.position - currentTarget;
            //    lookAtEvaluatedPosition = lookAtEvaluatedPosition.normalized * (currentMoveHeight > 0 ? 1 : -1);
            //    lookAtEvaluatedPosition.y = -GetCurveTangent(currentAnimationCurve, positionLerpValue);
            //    transform.forward = (transform.position + lookAtEvaluatedPosition);
            //    //lookAheadTimer = 0;
            //}

            transform.position = evaluatedPosition;

            elapsedTimeInPhase += Time.deltaTime;
            //lookAheadTimer += Time.deltaTime;

            yield return null;
        }
    }

    private void OnDestroy()
    {
        foreach(BodyPart b in bodies)
        {
            if(b.body != null)
            {
                Destroy(b.body);
            }
        }

        bodies.Clear();
    }
    


    private class BodyPart
    {
        public GameObject body;
        public Transform bodyInfront;
        public Queue<Vector3> targets = new();
        public Queue<AnimationCurve> animationCurves = new();
        public Queue<float> moveHeights = new();
        public Vector3 lastTarget;
        public float moveSpeed;
        public bool isFinished = false;
        public IEnumerator BodyUpdateRoutine() {


            float elapsedTimeInPhase = 0;
            float distance = 0;
            float positionLerpValue = 0;
            float currentMoveHeight = moveHeights.Dequeue();
            Vector3 currentTarget = targets.Dequeue();
            AnimationCurve currentAnimationCurve = animationCurves.Dequeue();

            float adjustedMoveSpeed = moveSpeed / (currentTarget - lastTarget).magnitude;

            while (true)
            {
                if (positionLerpValue >= 1)
                {
                    lastTarget = currentTarget;
                    elapsedTimeInPhase = 0;

                    while(targets.Count == 0)
                    {
                        isFinished = true;
                        yield return null;
                    }

                    isFinished = false;

                    currentTarget = targets.Dequeue();

                    distance = (currentTarget - lastTarget).magnitude;

                    currentAnimationCurve = animationCurves.Dequeue();
                    currentMoveHeight = moveHeights.Dequeue();
                    adjustedMoveSpeed = moveSpeed / (distance == 0 ? 1 : distance);
                    elapsedTimeInPhase = 0;
                    positionLerpValue = 0;
                }

                positionLerpValue = elapsedTimeInPhase * adjustedMoveSpeed;

                float newHeight = currentAnimationCurve.Evaluate(positionLerpValue) * currentMoveHeight;
                Vector3 evaluatedPosition = Vector3.Lerp(lastTarget, currentTarget, positionLerpValue) + Vector3.up * newHeight;

                body.transform.position = evaluatedPosition;

                elapsedTimeInPhase += Time.deltaTime;

                yield return null;
            }
        }

        public IEnumerator LookAtRoutine()
        {
            while (true)
            {
                if (bodyInfront != null)
                {
                    body.transform.LookAt(bodyInfront.position);
                }
                yield return null;
            }
        }

    }
}
