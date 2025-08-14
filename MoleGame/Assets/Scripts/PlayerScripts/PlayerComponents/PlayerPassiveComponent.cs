using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerPassiveComponent : IPlayerComponent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private List<IPlayerPassive> passives = new();
    private Coroutine passiveRoutine;
    internal override void Start()
    {
        base.Start();
        passiveRoutine = StartCoroutine(UpdatePassives());
    }
    public void RegisterPassive(IPlayerPassive passive)
    {
        passives.Add(passive);
        passive.SetPlayer(this);
        passive.OnStart();
    }
    public IEnumerator UpdatePassives()
    {
        while (true)
        {
            foreach (var passive in passives)
            {
                if (passive != null)
                {
                    passive.OnUpdate();
                }
            }

            yield return null;
        }
    }
    internal override void ResetStatsOnRespawn()
    {
        foreach (var passive in passives)
        {
            if(passive != null)
            {
                passive.OnEnd();
                Destroy(passive);
            }
        }
        passives.Clear();

        if (passiveRoutine == null)
        {
            passiveRoutine = StartCoroutine(UpdatePassives());
        }

        base.ResetStatsOnRespawn();
    }

    internal override void OnDestroy()
    {
        foreach (var passive in passives)
        {
            passive.OnEnd();
            Destroy(passive);
        }
        passives.Clear();
        base.OnDestroy();
    }
}
