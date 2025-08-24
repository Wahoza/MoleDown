using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenScript : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] GameObject LegL;
    [SerializeField] GameObject LegR;
    [SerializeField] Transform modelHolder;
    [SerializeField] Vector3 maxLegAngle;
    [SerializeField] Vector3 minLegAngle;
    [SerializeField] float legMoveTime;
    [SerializeField] float animationSpeed;

    [SerializeField] AnimationCurve animEasing;
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float range;
    [SerializeField] Rigidbody rb;
    
    private Transform target;
   
    private void Start()
    {
        ObjectManager.Instance.RegisterObject(gameObject);

        var players = GameManager.Instance.GetAlivePlayers();

        if(players.Count <= 0)
            return;

        var configTarget = players[Random.Range(0, players.Count)];

        var possibleTargets = GameManager.Instance.GetAllObjectsOfPlayerconfig(configTarget);
        
        List<Transform> aliveTargets = new ();
        foreach ( var possibleTarget in possibleTargets)
        {
            if (!possibleTarget.GetComponent<PlayerStateComponent>().IsDead())
            {
                aliveTargets.Add(possibleTarget);
            }
        }
        
        if(aliveTargets.Count <= 0)
            return;

        target = aliveTargets[Random.Range(0, aliveTargets.Count)];
        range *= target.transform.localScale.x;

        StartCoroutine(WalkRoutine());

        StartCoroutine(LookAtRoutine());

        StartCoroutine(WalkToRoutine());
    }
    private IEnumerator WalkToRoutine()
    {
        bool isInRange = false;
        while (!isInRange)
        {
            if (target != null)
            {


                var direction = target.position - transform.position;
                var yVelocity = GetComponent<Rigidbody>().linearVelocity.y;
                direction.y = 0;

                GetComponent<Rigidbody>().linearVelocity = direction.normalized * moveSpeed + Vector3.up * yVelocity;

                if (Vector3.Magnitude(transform.position - target.position) < range * target.localScale.x)
                {
                    isInRange = true;
                    GetComponent<DoActionOnCallComponent>().DoActions();
                }
            }

            yield return null;
        }
    }
    private IEnumerator LookAtRoutine()
    {
        while (true)
        {
            if (target != null)
            {
                var lookAt = target.position;
                lookAt.y = transform.position.y;

                if(modelHolder != null)
                {
                    modelHolder.LookAt(lookAt);
                }
            }
            yield return null;
        }
    }
    private IEnumerator WalkRoutine()
    {
        while (true)
        {
            StartCoroutine(RotateRoutine(LegL, minLegAngle, maxLegAngle, legMoveTime));
            yield return StartCoroutine(RotateRoutine(LegR, maxLegAngle, minLegAngle, legMoveTime));

            StartCoroutine(RotateRoutine(LegR, minLegAngle, maxLegAngle, legMoveTime));
            yield return StartCoroutine(RotateRoutine(LegL, maxLegAngle, minLegAngle, legMoveTime));
        }
    }
    IEnumerator RotateRoutine(GameObject goToAnimate, Vector3 from, Vector3 to, float time)
    {

        float elapsed = 0f;

        while (elapsed < time / Mathf.Clamp(animationSpeed, 0.0001f, float.MaxValue))
        {

            float t = elapsed / time;
            t = animEasing.Evaluate(t);

            goToAnimate.transform.localRotation = Quaternion.Euler(Vector3.Lerp(from, to, t));

            elapsed += Time.deltaTime * animationSpeed;


            yield return null;
        }

        goToAnimate.transform.localRotation = Quaternion.Euler(to);
    }
}
