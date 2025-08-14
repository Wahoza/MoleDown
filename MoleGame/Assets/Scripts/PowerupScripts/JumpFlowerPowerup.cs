using System.Collections;
using DG.Tweening;
using UnityEngine;

public class JumpFlowerPowerup : IPowerupScript
{
    [SerializeField] float maxY;
    [SerializeField] float minY;
    [SerializeField] float moveTime;
    [SerializeField] Vector3 rotationOverTime;
    [SerializeField] IPlayerPassive passiveEffect;
    [SerializeField] string particleEffect;
    [SerializeField] Transform transformToAnimate;

    internal override void Start()
    {
        base.Start();
        StartCoroutine(DoMoveAnim());
        StartCoroutine(RotationRoutine());
    }

    private IEnumerator RotationRoutine()
    {
        while (true)
        {
            transformToAnimate.Rotate(rotationOverTime * Time.deltaTime);
            yield return null;
        }
    }
    private IEnumerator DoMoveAnim()
    {
        while (true)
        {
            yield return transformToAnimate.DOLocalMoveY(maxY, moveTime).SetEase(Ease.OutSine).WaitForCompletion();
            yield return transformToAnimate.DOLocalMoveY(minY, moveTime).SetEase(Ease.OutSine).WaitForCompletion();
        }
    }

    internal override void DoPowerupEffect(Transform effectTransform)
    {
        Debug.Log("Powring up!");
        if (effectTransform == null)
        {
            return;
        }

        var inputHandler = effectTransform.gameObject.GetComponent<PlayerInputHandler>();

        if (inputHandler == null)
        {
            return;
        }

        var allPlayers = GameManager.Instance.GetAllObjectsOfPlayerconfig(inputHandler.playerConfig);

        foreach (var config in allPlayers)
        {
            if (config.GetComponent<PlayerPassiveComponent>() is PlayerPassiveComponent playerPassive && playerPassive != null)
            {
                var passive = Instantiate(passiveEffect);
                playerPassive.RegisterPassive(passive);
                ParticleManager.Instance.DoParticles(particleEffect, config.position);
            }
        }
    }
}
