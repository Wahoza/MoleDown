using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerPassiveComponent : IPlayerComponent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private List<IPlayerPassive> passives = new();
    private Coroutine passiveRoutine;
    internal override void Start()
    {
        GetComponent<PlayerActionHandler>().RegisterSpawnAction(ResetStatsOnRespawn);
        passiveRoutine = StartCoroutine(UpdatePassives());
    }
    public void RegisterPassive(IPlayerPassive passive)
    {
        Debug.Log("Registered New Passive");
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
        Debug.Log("Reset Passives");
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

    public T GetPassive<T>() where T : IPlayerPassive
    {
        return passives.OfType<T>().FirstOrDefault();
    }

    public List<IPlayerPassive> GetPlayerPassives()
    {
        return passives;
    }

    public void SetPlayerPassives(List<IPlayerPassive> newPassives)
    {
        foreach (var passive in passives)
        {
            passive.OnEnd();
            Destroy(passive);
        }
        passives.Clear();

        foreach (var passive in newPassives)
        {
            if(passive == null)
            {
                continue;
            }

            var passiveVariant = Instantiate(passive);

            passiveVariant.SetPlayer(this);

            passiveVariant?.OnStart();

            passives.Add(passiveVariant);
        }
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
