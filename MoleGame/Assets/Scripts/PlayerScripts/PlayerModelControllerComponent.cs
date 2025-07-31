using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerStateComponent))]

public class PlayerModelControllerComponent : MonoBehaviour
{
    [Header("ObjectsToAnimate")]
    [SerializeField] GameObject HandL;
    [SerializeField] GameObject HandR;
    [SerializeField] GameObject FootL;
    [SerializeField] GameObject FootR;
    [SerializeField] GameObject Body;
    [SerializeField] GameObject Face;
    [SerializeField] GameObject ModelHolder;

    [Header("Foot Animation Settings")]
    [SerializeField] Vector3 startFootRotation;
    [SerializeField] Vector3 endFootRotation;
    [SerializeField] float baseFootDuration;
    [SerializeField] float moveSpeedFootImpact;

    private Coroutine playerFootWalkRoutine;
    private Coroutine footLRoutine;
    private Coroutine footRRoutine;

    [Header("Hand Animation Settings")]
    [SerializeField] Vector3 startHandRotation;
    [SerializeField] Vector3 endHandRotation;
    [SerializeField] float baseHandDuration;
    [SerializeField] float moveSpeedHandImpact;

    private Coroutine playerHandWalkRoutine;
    private Coroutine handLRoutine;
    private Coroutine handRRoutine;

    [Header("Grab Animation Settings")]
    [SerializeField] Vector3 startGrabRotation;
    [SerializeField] Vector3 endGrabRotation;
    [SerializeField] float baseGrabDuration;

    private Coroutine playerGrabRoutine;
    private Coroutine playerReleaseRoutine;
    private bool lastGrabbed;

    [Header("Jump Animation Settings")]
    [SerializeField] Vector3 startJumpRotation;
    [SerializeField] Vector3 endJumpRotation;
    [SerializeField] float baseJumpDuration;
    [SerializeField] float jumpHoldDuration;

    private Coroutine playerJumpRoutine;

    [Header("Body Animation Settings")]
    [SerializeField] Vector3 startBodyPosition;
    [SerializeField] Vector3 endBodyPosition;
    [SerializeField] float bobAnimDuration;
    [SerializeField] float bobAnimMSImpact;

    private Coroutine bodyAnimRoutine;
    private Coroutine bodyBobRoutine;

    [Header("Body Landing Animation Settings")]
    [SerializeField] Vector3 startLandBodyPosition;
    [SerializeField] Vector3 endLandBodyPosition;
    [SerializeField] float landAnimDuration;
    [SerializeField] float airTimeDurationImpact;
    [SerializeField] float landHoldDuration;
    [SerializeField] float popUpDuration;
    [SerializeField] float airTimeMin;

    private Coroutine bodyLandingRoutine;

    [Header("AnimationSettings")]
    [SerializeField] AnimationCurve animEasing = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float moveSpeedMin;
    [SerializeField] private float animationSpeed = 1;

    private float moveSpeed;
    private PlayerStateComponent playerState;
    private void Start()
    {
        playerState = GetComponent<PlayerStateComponent>();

        playerFootWalkRoutine = StartCoroutine(WalkFootRoutine()); 
        playerHandWalkRoutine = StartCoroutine(WalkHandRoutine());

        bodyAnimRoutine = StartCoroutine(HeadBobRoutine());

        GetComponent<PlayerActionHandler>().RegisterMoveAction(UpdateMoveSpeed);
        GetComponent<PlayerActionHandler>().RegisterGrabAction(OnGrab);
        GetComponent<PlayerActionHandler>().RegisterJumpAction(OnJump);
        GetComponent<PlayerActionHandler>().RegisterLandAction(OnLand);
    }
    private void OnDestroy()
    {
        GetComponent<PlayerActionHandler>().UnregisterMoveAction(UpdateMoveSpeed);
        GetComponent<PlayerActionHandler>().UnregisterGrabAction(OnGrab);
        GetComponent<PlayerActionHandler>().UnregisterJumpAction(OnJump);
        GetComponent<PlayerActionHandler>().UnregisterLandAction(OnLand);
    }
    IEnumerator HeadBobRoutine()
    {
        while (true)
        {
            bodyBobRoutine = StartCoroutine(LerpWalkPositionValue(Body, startBodyPosition, endBodyPosition, bobAnimDuration, bobAnimMSImpact));
            yield return bodyBobRoutine;


            bodyBobRoutine = StartCoroutine(LerpWalkPositionValue(Body, endBodyPosition, startBodyPosition, bobAnimDuration, bobAnimMSImpact));
            yield return bodyBobRoutine;
        }
    }
    IEnumerator WalkHandRoutine()
    {
        while (true)
        {
            // Forward
            handRRoutine = StartCoroutine(LerpWalkRotationValue(HandR, endHandRotation, startHandRotation, baseHandDuration, moveSpeedHandImpact));
            handLRoutine = StartCoroutine(LerpWalkRotationValue(HandL, startHandRotation, endHandRotation, baseHandDuration, moveSpeedHandImpact));
            yield return handLRoutine;

            // Backward
            handRRoutine = StartCoroutine(LerpWalkRotationValue(HandR, startHandRotation, endHandRotation, baseHandDuration, moveSpeedHandImpact));
            handLRoutine = StartCoroutine(LerpWalkRotationValue(HandL, endHandRotation, startHandRotation, baseHandDuration, moveSpeedHandImpact));
            yield return handLRoutine;
        }
    }
    IEnumerator WalkFootRoutine()
    {
        while (true)
        {
            // Forward
            footRRoutine = StartCoroutine(LerpWalkRotationValue(FootR, endFootRotation, startFootRotation, baseFootDuration, moveSpeedFootImpact));
            footLRoutine = StartCoroutine(LerpWalkRotationValue(FootL, startFootRotation, endFootRotation, baseFootDuration, moveSpeedFootImpact));

            yield return footLRoutine;
            // Backward
            footRRoutine = StartCoroutine(LerpWalkRotationValue(FootR, startFootRotation, endFootRotation, baseFootDuration, moveSpeedFootImpact));
            footLRoutine = StartCoroutine(LerpWalkRotationValue(FootL, endFootRotation, startFootRotation, baseFootDuration, moveSpeedFootImpact));

            yield return footLRoutine;
        }
    }

    IEnumerator LerpWalkRotationValue(GameObject goToAnimate, Vector3 from, Vector3 to, float time, float msImpact)
    {
        float elapsed = 0f;

        while (elapsed < time / Mathf.Clamp(animationSpeed, 0.0001f, float.MaxValue))
        {
            if (moveSpeed > moveSpeedMin && !playerState.IsStunned())
            {
                float t = elapsed / time;
                t = animEasing.Evaluate(t);

                goToAnimate.transform.localRotation = Quaternion.Euler(Vector3.Lerp(from, to, t));

                elapsed += Time.deltaTime * moveSpeed * msImpact * animationSpeed;
            }
            else
            {
                elapsed = 0.3f;

                goToAnimate.transform.localRotation = Quaternion.identity;
            }
                yield return null;
        }

        goToAnimate.transform.localRotation = Quaternion.Euler(to);
    }

    IEnumerator LerpWalkPositionValue(GameObject goToAnimate, Vector3 from, Vector3 to, float time, float msImpact)
    {
        float elapsed = 0f;

        while (elapsed < time / Mathf.Clamp(animationSpeed, 0.0001f, float.MaxValue))
        {
            if (moveSpeed > moveSpeedMin && !playerState.IsStunned())
            {
                float t = elapsed / time;
                goToAnimate.transform.localPosition = Vector3.Lerp(from, to, t);

                elapsed += Time.deltaTime * msImpact * moveSpeed * animationSpeed;
            }

            yield return null;
        }

        goToAnimate.transform.localPosition = to;
    }

    IEnumerator HandGrabRoutine()
    {
        handLRoutine = StartCoroutine(RotateRoutine(HandL, startGrabRotation, endGrabRotation, baseGrabDuration));
        handRRoutine = StartCoroutine(RotateRoutine(HandR, startGrabRotation, endGrabRotation, baseGrabDuration));

        yield return null;
    }

    IEnumerator HandReleaseRoutine()
    {
        handLRoutine = StartCoroutine(RotateRoutine(HandL, endGrabRotation, startGrabRotation, baseGrabDuration));
        handRRoutine = StartCoroutine(RotateRoutine(HandR, endGrabRotation, startGrabRotation, baseGrabDuration));

        yield return handRRoutine;

        if (playerHandWalkRoutine == null)
        {
            StartCoroutine(WalkHandRoutine());
        }
    }
    IEnumerator LerpPositionValue(GameObject goToAnimate, Vector3 from, Vector3 to, float time)
    {
        float elapsed = 0f;

        while (elapsed < time / Mathf.Clamp(animationSpeed, 0.0001f, float.MaxValue))
        {
            
            float t = elapsed / time;
            goToAnimate.transform.localPosition = Vector3.Lerp(from, to, t);

            elapsed += Time.deltaTime * animationSpeed;
            

            yield return null;
        }
        goToAnimate.transform.localPosition = to;
    }
    IEnumerator LandRoutine(float animTime) 
    {
        yield return StartCoroutine(LerpPositionValue(Body, startLandBodyPosition, endLandBodyPosition, landAnimDuration));

        if (animationSpeed != 0f)
        {
            yield return new WaitForSeconds((landHoldDuration + animTime) / animationSpeed);
        }
        else
        {
            yield return new WaitForSeconds(float.MaxValue);
        }

        yield return StartCoroutine(LerpPositionValue(Body, endLandBodyPosition, startLandBodyPosition, popUpDuration));

        bodyLandingRoutine = null;

        if (bodyAnimRoutine == null)
        {
            bodyAnimRoutine = StartCoroutine(HeadBobRoutine());
        }
    }
    IEnumerator RotateRoutine(GameObject goToAnimate, Vector3 from, Vector3 to, float time) { 
        
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
    void UpdateMoveSpeed(Vector3 movement)
    {
        Vector2 adjusted = new (movement.x, movement.z);
        moveSpeed = Mathf.Abs(adjusted.magnitude);
    }

    private IEnumerator JumpRoutine() 
    {
        StartCoroutine(RotateRoutine(FootL, startJumpRotation, endJumpRotation, baseJumpDuration));

        yield return StartCoroutine(RotateRoutine(FootR, startJumpRotation, endJumpRotation, baseJumpDuration));

        if (animationSpeed != 0f)
        {
            yield return new WaitForSeconds(jumpHoldDuration / animationSpeed);
        }
        else
        {
            yield return new WaitForSeconds(float.MaxValue);
        }

        if (playerFootWalkRoutine == null)
        {
            playerFootWalkRoutine = StartCoroutine(WalkFootRoutine());
        }

        playerJumpRoutine = null;
    }
    void OnGrab(bool grabbed)
    {
        Debug.Log("Grab " + grabbed);

        if (grabbed != lastGrabbed)
        {
            if (grabbed)
            {
                if (playerHandWalkRoutine != null)
                {
                    StopCoroutine(playerHandWalkRoutine);

                    playerHandWalkRoutine = null;
                }
                if (handLRoutine != null)
                {
                    StopCoroutine(handLRoutine);
                    StopCoroutine(handRRoutine);
                }
                if (playerReleaseRoutine != null)
                {
                    StopCoroutine(playerReleaseRoutine);
                }

                playerGrabRoutine = StartCoroutine(HandGrabRoutine());

                lastGrabbed = grabbed;
            }
            else
            {
                if (handLRoutine != null)
                {
                    StopCoroutine(handLRoutine);
                    StopCoroutine(handRRoutine);
                }

                playerReleaseRoutine = StartCoroutine(HandReleaseRoutine());

                lastGrabbed = grabbed;
            }

            lastGrabbed = grabbed;

        }
    }

    void OnJump()
    {
        if(playerJumpRoutine == null)
        {
            if (playerFootWalkRoutine != null)
            {
                StopCoroutine(playerFootWalkRoutine);
                playerFootWalkRoutine = null;
            }

            if (footLRoutine != null)
            {
                StopCoroutine(footLRoutine);
                StopCoroutine(footRRoutine);

                footLRoutine = null;
                footRRoutine = null;
            }

            playerJumpRoutine = StartCoroutine(JumpRoutine());
        }
    }

    void OnLand(float airTime) 
    {
        if(airTime <= airTimeMin)
            return;

        Debug.Log("AirTime + " + airTime);
        airTime -= airTimeMin;
        if(bodyLandingRoutine == null)
        {
            if (bodyAnimRoutine != null)
            {
                StopCoroutine(bodyAnimRoutine);
                bodyAnimRoutine = null;
            }

            if(bodyBobRoutine != null)
            {
                StopCoroutine (bodyBobRoutine);
                bodyBobRoutine = null;
            }

            bodyLandingRoutine = StartCoroutine(LandRoutine(airTime * airTimeDurationImpact));
        }
    }
}

