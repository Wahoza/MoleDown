using DG.Tweening;
using UnityEngine;
using System.Collections;


public class CrownPowerup : IPowerupScript
{
    [Header("Animation")]
    [SerializeField] float maxY;
    [SerializeField] float minY;
    [SerializeField] float moveTime;
    [SerializeField] Vector3 rotationOverTime;
    [SerializeField] string particleEffect;
    [SerializeField] Transform transformToAnimate;

    [Header("Spawn StandArea")]
    [SerializeField] GameObject standablePrefab;
    [SerializeField] float onSpawnCheckDist;
    [SerializeField] float standableYLevel;
    [SerializeField] LayerMask groundLayer;

    [Header("Respawn Crown")]
    [SerializeField] float minYLevel;
    private bool crownedPlayer;
    private bool isQuit = false;
    internal override void Start()
    {
        base.Start();

        if (!Physics.Raycast(transform.position, Vector3.down, onSpawnCheckDist, groundLayer))
        {
            var platform = Instantiate(standablePrefab, position: new Vector3(transform.position.x, standableYLevel, transform.position.z), Quaternion.identity);
        }

        StartCoroutine(DoMoveAnim());
        StartCoroutine(RotationRoutine());
        StartCoroutine(CheckYLevel());
        crownedPlayer = false;
    }
    private IEnumerator CheckYLevel()
    {
        while (true)
        {
            if(transform.position.y < minYLevel)
            {
                GameManager.Instance.RequestCrownSpawn();
                Destroy(gameObject);
            }
            yield return null;
        }
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
        base.DoPowerupEffect(effectTransform);

        ParticleManager.Instance.DoParticles(particleEffect, effectTransform.position);

        GameManager.Instance.RequestCrownTransfer(effectTransform);

        crownedPlayer = true;
    }
}
