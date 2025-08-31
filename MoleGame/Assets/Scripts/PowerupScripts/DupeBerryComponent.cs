using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;

public class DupeBerryComponent : IPowerupScript
{
    [SerializeField] float maxY;
    [SerializeField] float minY;
    [SerializeField] float moveTime;
    [SerializeField] Vector3 rotationOverTime;
    [SerializeField] float spawnDistance;
    [SerializeField] Transform transformToAnimate;
    [SerializeField] string particleEffect;
    [SerializeField] int maxCloneCount;
    [SerializeField] string dupeAudioName;
    internal override void Start()
    {
        base.Start();
        StartCoroutine(DoMoveAnim());
        StartCoroutine(RotationRoutine());
    }

    private IEnumerator RotationRoutine()
    {
        while (true) {
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
        if(effectTransform == null)
        {
            return;
        }

        var inputHandler = effectTransform.gameObject.GetComponent<PlayerInputHandler>();

        if (inputHandler == null) {
            return; 
        }

        var allPlayers = GameManager.Instance.GetAllObjectsOfPlayerconfig(inputHandler.playerConfig).Where(player => !player.GetComponent<PlayerStateComponent>().IsDead()).ToList();

        GetComponent<SphereCollider>().enabled = false;
        int createdClones = 0;
        foreach (var config in allPlayers)
        {
            if(maxCloneCount < createdClones + allPlayers.Count)
            {
                return;
            }
            if (config.GetComponent<PlayerInputHandler>() is PlayerInputHandler playerI && playerI != null)
            {
                if(config.GetComponent<PlayerStateComponent>().IsGrabbed())
                    { continue; }

                var clone = Instantiate(config);
                clone.position = config.forward * config.localScale.x * spawnDistance + config.position;
                clone.GetComponent<PlayerInputHandler>().Initialize(playerI.playerConfig, true);
                ParticleManager.Instance.DoParticles(particleEffect, clone.position);
                
                AudioManager.Instance.Play3DAudioAtPosition(dupeAudioName, clone.position, true);
                createdClones++;
            }
        }
    }
}
