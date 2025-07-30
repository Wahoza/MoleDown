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

    [Header("FootAnimationSettings")]
    [SerializeField] Vector3 startFootRotation;
    [SerializeField] Vector3 endFootRotation;
    [SerializeField] float baseFootDuration;
    [SerializeField] float moveSpeedFootImpact;

    private IEnumerator playerFootWalkRoutine;

    [Header("HandAnimationSettings")]
    [SerializeField] Vector3 startHandRotation;
    [SerializeField] Vector3 endHandRotation;
    [SerializeField] float baseHandDuration;
    [SerializeField] float moveSpeedHandImpact;

    private Coroutine playerHandWalkRoutine;
    private Coroutine handLRoutine;
    private Coroutine handRRoutine;

    [Header("GrabAnimationSettings")]
    [SerializeField] Vector3 startGrabRotation;
    [SerializeField] Vector3 endGrabRotation;
    [SerializeField] float baseGrabDuration;

    private Coroutine playerGrabRoutine;
    private Coroutine playerReleaseRoutine;
    private bool lastGrabbed;

    [Header("WalkAnimationSettings")]
    [SerializeField] AnimationCurve footEasing = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float moveSpeedMin;

    private float moveSpeed;

    [Header("ArmAnimationSettings")]
    private PlayerStateComponent playerState;
    private void Start()
    {
        playerState = GetComponent<PlayerStateComponent>();

        playerFootWalkRoutine = WalkFootRoutine(); 
        StartCoroutine(playerFootWalkRoutine);

        playerHandWalkRoutine = StartCoroutine(WalkHandRoutine());

        GetComponent<PlayerActionHandler>().RegisterMoveAction(UpdateMoveSpeed);
        GetComponent<PlayerActionHandler>().RegisterGrabActions(OnGrab);
    }
    private void OnDestroy()
    {
        GetComponent<PlayerActionHandler>().UnregisterMoveAction(UpdateMoveSpeed);
        GetComponent<PlayerActionHandler>().UnregisterGrabActions(OnGrab);
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
            StartCoroutine(LerpWalkRotationValue(FootR, endFootRotation, startFootRotation, baseFootDuration, moveSpeedFootImpact));
            yield return StartCoroutine(LerpWalkRotationValue(FootL, startFootRotation, endFootRotation, baseFootDuration, moveSpeedFootImpact));

            // Backward
            StartCoroutine(LerpWalkRotationValue(FootR, startFootRotation, endFootRotation, baseFootDuration, moveSpeedFootImpact));
            yield return StartCoroutine(LerpWalkRotationValue(FootL, endFootRotation, startFootRotation, baseFootDuration, moveSpeedFootImpact));
        }
    }

    IEnumerator LerpWalkRotationValue(GameObject goToAnimate, Vector3 from, Vector3 to, float time, float msImpact)
    {
        float elapsed = 0f;

        while (elapsed < time)
        {
            if (moveSpeed > moveSpeedMin)
            {
                float t = elapsed / time;
                t = footEasing.Evaluate(t);

                goToAnimate.transform.localRotation = Quaternion.Euler(Vector3.Lerp(from, to, t));

                elapsed += Time.deltaTime * moveSpeed * msImpact;
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

    IEnumerator LerpPositionValue(GameObject goToAnimate, Vector3 from, Vector3 to, float time)
    {
        float elapsed = 0f;

        while (elapsed < time)
        {
            float t = elapsed / time;
            goToAnimate.transform.localPosition = Vector3.Lerp(from, to, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        goToAnimate.transform.position = to;
    }

    IEnumerator HandGrabRoutine()
    {
        handLRoutine = StartCoroutine(GrabRoutine(HandL, startGrabRotation, endGrabRotation, baseGrabDuration));
        handRRoutine = StartCoroutine(GrabRoutine(HandR, startGrabRotation, endGrabRotation, baseGrabDuration));

        yield return null;
    }

    IEnumerator HandReleaseRoutine()
    {
        handLRoutine = StartCoroutine(GrabRoutine(HandL, endGrabRotation, startGrabRotation, baseGrabDuration));
        handRRoutine = StartCoroutine(GrabRoutine(HandR, endGrabRotation, startGrabRotation, baseGrabDuration));

        yield return handRRoutine;

        if (playerHandWalkRoutine == null)
        {
            StartCoroutine(WalkHandRoutine());
        }
    }
    IEnumerator GrabRoutine(GameObject goToAnimate, Vector3 from, Vector3 to, float time) { 
        
        float elapsed = 0f;

        while (elapsed < time)
        {
            
            float t = elapsed / time;
            t = footEasing.Evaluate(t);

            goToAnimate.transform.localRotation = Quaternion.Euler(Vector3.Lerp(from, to, t));

            elapsed += Time.deltaTime;
        

            yield return null;
        }

        goToAnimate.transform.localRotation = Quaternion.Euler(to);
    }
    void UpdateMoveSpeed(Vector3 movement)
    {
        Vector2 adjusted = new (movement.x, movement.z);
        moveSpeed = Mathf.Abs(adjusted.magnitude);
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
}

