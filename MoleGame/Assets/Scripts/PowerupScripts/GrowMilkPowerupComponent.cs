using DG.Tweening;
using UnityEngine;
using System.Collections;
public class GrowMilkPowerupComponent : IPowerupScript
{
    [SerializeField] float maxY;
    [SerializeField] float minY;
    [SerializeField] float moveTime;
    [SerializeField] Vector3 rotationOverTime;
    [SerializeField] string particleEffect;
    [SerializeField] Transform transformToAnimate;

    [Header("Effects (Additive)")]
    [SerializeField] IPlayerPassive passiveEffect;
    [SerializeField] float radiusScaleIncrease;
    [SerializeField] float scaleChange;
    [SerializeField] float massChange;
    [SerializeField] float jumpStrengthChange;
    [SerializeField] float maxJumpVelChange;
    [SerializeField] float landTerrainDeformRadiusChange;
    [SerializeField] float landTerrainDeformIncreaseChange;
    [SerializeField] float pushStrengthChange;
    [SerializeField] float grabTerrainRadiusIncreaseChange;
    [SerializeField] float deformOnAttackRadiusIncrease;
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

                if (passive is DestroyTerrainOnDistanceWalkPassive p)
                {
                    //p.offset *= config.localScale.x;
                    p.deformRadius *= Mathf.Clamp(config.localScale.x * radiusScaleIncrease, 1, 1000);
                    playerPassive.RegisterPassive(p);
                }
                ParticleManager.Instance.DoParticles(particleEffect, config.position);

                config.localScale += Vector3.one * scaleChange;
                config.GetComponent<Rigidbody>().mass += massChange;
                config.GetComponent<PlayerJumpComponent>().currentJumpStrength += jumpStrengthChange;
                config.GetComponent<PlayerJumpComponent>().currentMaxJumpVelocity += maxJumpVelChange;
                config.GetComponent<PlayerLandingDestructionComponent>().currentBaseRadius += landTerrainDeformRadiusChange;
                config.GetComponent<PlayerLandingDestructionComponent>().currentRadiusIncrease += landTerrainDeformIncreaseChange;
                config.GetComponent<PlayerAttackComponent>().currentPushForce += pushStrengthChange;
                config.GetComponent<PlayerGrabTerrainComponent>().currentRadiusIncrease += grabTerrainRadiusIncreaseChange;
                config.GetComponent<PlayerGrabComponent>().currentThrowForce += pushStrengthChange;
                config.GetComponent<PlayerAttackComponent>().currentTerrainModRange += deformOnAttackRadiusIncrease;
            }
        }
    }
}
