using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using DG.Tweening;
using System;
using UnityEngine.InputSystem;
public class TentacleObstacle : IGroundedObstacle
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("ObjectSetup")]
    [SerializeField] Transform tentacleHolder;
    [SerializeField] Transform firstRBHolder;
    [SerializeField] Transform tip;
    [SerializeField] Transform stickyPrefab;
    [SerializeField] SphereCollider attackTriggerArea;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] List<Transform> triggers;
    [SerializeField] float minScale;
    [SerializeField] float maxScale;

    [Header("Idle Perlin Setup")]
    [SerializeField] float maxVelocity;
    [SerializeField] float perlinEffectVelocity;
    [SerializeField] float perlinScrollSpeed;
    [SerializeField] float maxNoiseOffset;
    [SerializeField] float jointOffset;
    [SerializeField] float jointUpVelocity;
    [SerializeField] float minJointUpVelocityHeight;
    [SerializeField] float maxStartOffset;
    [SerializeField] float minIdleDuration;

    [Header("Attack")]
    [SerializeField] AnimationCurve attackUpAnimation;
    [SerializeField] AnimationCurve attackForwardAnimation;
    [SerializeField] float attackUpHeight;
    [SerializeField] float attackDuration;
    [SerializeField] float attackPredictionAmount;
    [SerializeField] float lookAtDuration;
    [SerializeField] float targetMoveOffset;
    [SerializeField] float refreshWaitTime;

    [Header("Attachment")]
    [SerializeField] int maxStartJumps;
    [SerializeField] int jumpsRequiredToEscape;
    [SerializeField] float maxDistAwayFromTentacle;
    [SerializeField] float maxDistAwayOnGrab;
    [SerializeField] float attackRangeOnGrab;
    private Transform grabber;

    private Queue<Transform> targetTransforms = new();
    private List<Coroutine> idleTentaclesPhysicRoutines = new();

    private Rigidbody[] rbs;
    private List<float> initYOffsets = new();

    private List<StuckPlayerSetup> stuckPlayers = new();

    private Vector2 xnoiseOffset;
    private Vector2 znoiseOffset;
    private bool isAttacking;

    private Coroutine tentacleRoutine;
    internal override void Start()
    {
        if (ObjectManager.Instance != null)
        {
            base.Start();
        }
        tentacleHolder.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        transform.localScale = Vector3.one * UnityEngine.Random.Range(minScale, maxScale);
        rbs = firstRBHolder.GetComponentsInChildren<Rigidbody>();

        if (rbs.Count() <= 0)
        {
            return;
        }

        int index = 0;
        foreach (var rb in rbs)
        {
            var initLocalPos = rb.transform.localPosition;
            Vector2 startOffset = Vector2.zero;
            startOffset.x = Mathf.PerlinNoise(xnoiseOffset.x, xnoiseOffset.y + index * jointOffset) - 0.5f;
            startOffset.y = Mathf.PerlinNoise(znoiseOffset.x, znoiseOffset.y + index * jointOffset) - 0.5f;
            startOffset *= maxStartOffset;

            initLocalPos.x += startOffset.x;
            initLocalPos.z += startOffset.y;

            rb.transform.localPosition = initLocalPos;

            initYOffsets.Add(transform.localPosition.y + minJointUpVelocityHeight);

            index++;
        }

        if (ObjectManager.Instance != null)
        {
            tentacleRoutine = StartCoroutine(TentacleCycle());
        }
    }

    internal override void OnGroundInvalidate()
    {
        StopAllCoroutines();

        foreach (var trigger in triggers)
        {
            trigger.gameObject.SetActive(false);
        }

        foreach (var stuckPlayer in stuckPlayers)
        {
            if (stuckPlayer.player != null)
            {
                stuckPlayer.player.GetComponent<PlayerStateComponent>().DisableRagdoll();

                stuckPlayer.player.GetComponent<PlayerInputHandler>().UnregisterInputJumpFuncs(stuckPlayer.jumpAction);

                Destroy(stuckPlayer.joint);
            }
        }

        foreach(var rb in rbs)
        {
            rb.useGravity = true;
        }
        firstRBHolder.parent.GetComponent<Rigidbody>().isKinematic = false;

        stuckPlayers.Clear();
    }
    private IEnumerator TentacleCycle()
    {
        while (true)
        {
            isAttacking = false;

            xnoiseOffset = UnityEngine.Random.value * maxNoiseOffset * Vector2.one;
            znoiseOffset = UnityEngine.Random.value * maxNoiseOffset * Vector2.one;

            foreach (var rb in rbs)
            {
                rb.useGravity = false;
            }

            for (int i = 0; i < rbs.Length; i++)
            {
                idleTentaclesPhysicRoutines.Add(StartCoroutine(IdleTentaclePhysicRoutine(rbs[i], i)));
            }

            yield return new WaitForSeconds(minIdleDuration);

            foreach(var collider in Physics.OverlapSphere(attackTriggerArea.transform.position + attackTriggerArea.center, attackTriggerArea.radius, playerLayer))
            {
                if(collider.GetComponent<PlayerActionHandler>() != null)
                {
                    DoAttack(collider.transform);
                }
            }

            yield return new WaitUntil(() => isAttacking);

            foreach (var crt in idleTentaclesPhysicRoutines)
            {
                if (crt != null)
                {
                    StopCoroutine(crt);
                }
            }

            idleTentaclesPhysicRoutines.Clear();

            foreach (var rb in rbs)
            {
                rb.useGravity = true;
            }

            while(targetTransforms.Count > 0)
            {
                yield return StartCoroutine(AttackRoutine(targetTransforms.Dequeue()));

                yield return new WaitForSeconds(refreshWaitTime);
            }
            
            while(stuckPlayers.Count > 0)
            {
                yield return null;
            }
        }
    }

    private IEnumerator IdleTentaclePhysicRoutine(Rigidbody rb, int index)
    {
        float elapsedTime = 0;

        var initLocalPos = rb.transform.localPosition;

        float minHeight = initYOffsets[index];

        while (true)
        {
            float xCurrentNoise = Mathf.PerlinNoise(xnoiseOffset.x, xnoiseOffset.y + elapsedTime * perlinScrollSpeed + index * jointOffset);
            xCurrentNoise -= 0.5f;
            float zCurrentNoise = Mathf.PerlinNoise(znoiseOffset.x, znoiseOffset.y + elapsedTime * perlinScrollSpeed + index * jointOffset);
            zCurrentNoise -= 0.5f;

            rb.AddForce(new Vector3 ( xCurrentNoise * perlinEffectVelocity, jointUpVelocity, zCurrentNoise * perlinEffectVelocity ) * Time.deltaTime, ForceMode.Force);

            if (rb.transform.position.y < minHeight)
            {
                rb.AddForce(Vector3.up * jointUpVelocity * Time.deltaTime, ForceMode.Force);
            }

            if (rb.linearVelocity.x * rb.linearVelocity.x + rb.linearVelocity.z * rb.linearVelocity.z > Mathf.Pow(maxVelocity, 2))
            {
                var newVelocity = rb.linearVelocity.normalized * maxVelocity;
                newVelocity.y = rb.linearVelocity.y;
                rb.linearVelocity = newVelocity;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void DoAttack(Transform target) {
        Debug.Log("Attack Initiated");
        if(target == grabber)
        {
            return;
        }

        if (stuckPlayers.Any(setup => setup.player == transform))
        {
            return;
        }

        targetTransforms.Enqueue(target);
        isAttacking = true;
    }

    private IEnumerator AttackRoutine(Transform target)
    {
        float elapsedTime = 0;
        float phase = 0;

        Vector3 targetPos = target.position;
        tentacleHolder.DOLookAt(new Vector3(targetPos.x, tentacleHolder.position.y, targetPos.z), lookAtDuration);

        if (target.GetComponent<Rigidbody>() is var targetRb && targetRb != null)
        {
            targetPos += targetRb.linearVelocity * attackDuration * attackPredictionAmount;
        }

        Vector3 origin = tip.position;
        Vector3 moveVecFromOrigin = targetPos - origin;

        float moveVecLength = moveVecFromOrigin.magnitude + targetMoveOffset - target.localScale.x;
        moveVecFromOrigin = moveVecFromOrigin.normalized;

        while (elapsedTime < attackDuration)
        {
            elapsedTime += Time.deltaTime;
            phase = elapsedTime / attackDuration;

            float forwardSample = attackForwardAnimation.Evaluate(phase);
            float heightSample = attackUpAnimation.Evaluate(phase);

            Vector3 newPosition = origin;
            newPosition += moveVecFromOrigin * forwardSample * moveVecLength;
            newPosition += Vector3.up * heightSample * attackUpHeight;

            tip.GetComponent<Rigidbody>().MovePosition(newPosition);
            //tip.position = newPosition;

            yield return null;
        }

        yield return null;
    }

    public void DoStick(Rigidbody stickTarget)
    {
        if(grabber == stickTarget)
        {
            return;
        }
        if (stuckPlayers.Any(setup => setup.player == stickTarget.transform))
        {
            return;
        }

        Debug.Log("Stick New!");

        var stickyJoint = stickyPrefab.GetComponent<ConfigurableJoint>();

        if(stickyJoint == null)
            return;


        StuckPlayerSetup stuckPlayer = new();

        stickTarget.transform.position = (tip.position);

        stuckPlayer.joint = tip.gameObject.AddComponent<ConfigurableJoint>();

        stuckPlayer.joint.enablePreprocessing = stickyJoint.enablePreprocessing;
        stuckPlayer.joint.linearLimit = stickyJoint.linearLimit;
        stuckPlayer.joint.xMotion = stickyJoint.xMotion;
        stuckPlayer.joint.yMotion = stickyJoint.yMotion;
        stuckPlayer.joint.zMotion = stickyJoint.zMotion;
        stuckPlayer.joint.angularXMotion = stickyJoint.angularXMotion;
        stuckPlayer.joint.angularYMotion = stickyJoint.angularYMotion;
        stuckPlayer.joint.angularZMotion = stickyJoint.angularZMotion;

        stuckPlayer.joint.connectedBody = stickTarget;


        stuckPlayer.player = stickTarget.transform;

        stuckPlayer.jumpCount = UnityEngine.Random.Range(0, maxStartJumps + 1);

        stuckPlayers.Add(stuckPlayer);

        stuckPlayer.jumpAction = (ctx) => OnEscapeJump(stuckPlayer);
        stickTarget.GetComponent<PlayerStateComponent>().EnableRagdoll();
        stickTarget.GetComponent<PlayerInputHandler>().RegisterInputJumpFuncs(stuckPlayer.jumpAction);

        stuckPlayer.compareMaxDist = StartCoroutine(CompareMaxDistanceForTransform(stuckPlayer));
    }
    private IEnumerator CompareMaxDistanceForTransform(StuckPlayerSetup player)
    {
        bool finished = false;
        while (!finished)
        {
            if(player.player == null)
                yield break;

            if(Vector3.SqrMagnitude(player.player.position - firstRBHolder.position) > Mathf.Pow(maxDistAwayFromTentacle, 2))
            {
                player.player.GetComponent<PlayerInputHandler>().UnregisterInputJumpFuncs(player.jumpAction);

                player.player.GetComponent<PlayerStateComponent>().DisableRagdoll();

                Destroy(player.joint);

                stuckPlayers.Remove(player);

                finished = true;
            }
            yield return null;
        }
    }
    private void OnEscapeJump(StuckPlayerSetup stuckPlayer)
    {
        stuckPlayer.jumpCount++;

        if (jumpsRequiredToEscape < stuckPlayer.jumpCount)
        {
            stuckPlayer.player.GetComponent<PlayerInputHandler>().UnregisterInputJumpFuncs(stuckPlayer.jumpAction);

            stuckPlayer.player.GetComponent<PlayerStateComponent>().DisableRagdoll();

            stuckPlayers.Remove(stuckPlayer);
            Destroy(stuckPlayer.joint);
        }
    }
    public override void OnTerrainGrabbedSetPocket(Transform pocketTransform)
    {
        base.OnTerrainGrabbedSetPocket(pocketTransform);

        attackTriggerArea.radius = attackRangeOnGrab;

        grabber = pocketTransform.parent.parent;

        Debug.Log("Grabber name: " + grabber.gameObject.name);

        if(stuckPlayers.Find(playerSetup => playerSetup.player == grabber) is var match && match != null)
        {
            if(Vector3.SqrMagnitude(match.player.position - firstRBHolder.position) > Mathf.Pow(maxDistAwayOnGrab, 2))
            {
                match.player.GetComponent<PlayerInputHandler>().UnregisterInputJumpFuncs(match.jumpAction);

                match.player.GetComponent<PlayerStateComponent>().DisableRagdoll();

                Destroy(match.joint);

                stuckPlayers.Remove(match);
            }
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();

        foreach(var stuckPlayer in stuckPlayers)
        {
            if(stuckPlayer.player != null)
            {
                stuckPlayer.player.GetComponent<PlayerStateComponent>().DisableRagdoll();

                stuckPlayer.player.GetComponent<PlayerInputHandler>().UnregisterInputJumpFuncs(stuckPlayer.jumpAction);

                Destroy(stuckPlayer.joint);
            }
        }
    }

    private class StuckPlayerSetup
    {
        public ConfigurableJoint joint;
        public Transform player;
        public int jumpCount;

        public Action<InputAction.CallbackContext> jumpAction;
        public Coroutine compareMaxDist;
    }
}
